using UnityEngine;
using System.Collections;
using VRStandardAssets.Utils;
using de.dfki.events;
using UnityEngine.UI;

public class TangoInputHandler : MonoBehaviour {

	[SerializeField] private TangoInput tangoInput;
	[SerializeField] private VRInteractiveItem interactiveItem;
	[SerializeField] private GameObject movingCubeModel;
    [SerializeField] private Renderer renderer;
	[SerializeField] private GameObject over;
	private GameObject movingCube;
	private int id;

	private bool annotated = false;
	private bool draggable = false;

	void Awake () {
		id = this.gameObject.GetComponent<MetaData>().id;
	}

	private void OnEnable()
	{
        interactiveItem.OnOver += HandleOver;
        interactiveItem.OnOut += HandleOut;
        tangoInput.OnSelect += OnSelect;
		tangoInput.OnRotateLeft += OnRotateLeft;
		tangoInput.OnRotateRight += OnRotateRight;
	}

	private void OnDisable()
    {
        interactiveItem.OnOver -= HandleOver;
        interactiveItem.OnOut -= HandleOut;
        tangoInput.OnSelect -= OnSelect;
		tangoInput.OnRotateLeft -= OnRotateLeft;
		tangoInput.OnRotateRight -= OnRotateRight;
	}

    //Handle the Over event
    private void HandleOver()
    {
		renderer.material.color = Color.yellow;
		over.SetActive(true);
    }


    //Handle the Out event
    private void HandleOut()
    {
		if (!draggable && !annotated)
			renderer.material.color = Color.green;
		over.SetActive(false);
    }

    void OnSelect(){
		switch (TangoUI.currentTool){
			case TangoUI.Tool.DRAGNDROP:
				OnDragDrop();
				break;
			case TangoUI.Tool.ANNOTATE:
				OnAnnotate();
				break;
			case TangoUI.Tool.TEAMSPEAK:
				OnTeamSpeak();
				break;
			case TangoUI.Tool.DRAGROTATE:
				OnDragRotate();
				break;
		}
	}

	void OnDragDrop(){
		Debug.Log("started drag drop");
		if(interactiveItem.IsOver){
			Debug.Log("is Over");
			if(movingCube == null && !draggable){
				if(RPCClient.client.Can_Interact(id)){
					draggable = true;
					renderer.material.color = Color.red;
					RPCClient.client.LockGameObject(id);
					movingCube = Instantiate(movingCubeModel);
					movingCube.transform.rotation = this.transform.rotation;
					movingCube.SetActive(true);
				}
			}
		} else if(movingCube != null && draggable){
			draggable = false;
			if(!annotated)
				renderer.material.color = Color.green;
			Vector3 pos = movingCube.transform.position;
			PositionEvent posEvent = new PositionEvent(Device.TANGO, ObjType.CUBE, new Position(pos.x, pos.y, pos.z), id);
			if(!RPCClient.client.Move(posEvent))
				Debug.Log("Could not move cube");
			Destroy(movingCube);
			movingCube = null;
		}
	}

	void OnAnnotate(){
		if(interactiveItem.IsOver){
			Annotation an = new Annotation(Device.TANGO, id);
			if (!RPCClient.client.Annotate(an))
			{
				Debug.Log("Annotation Failed");
			}
			annotated =true;
		}
	}

	void OnRotateLeft(){
		if(movingCube != null && draggable)
			movingCube.transform.RotateAround(movingCube.transform.position, Vector3.up, 10);
	}

	void OnRotateRight(){
		if(movingCube != null && draggable)
			movingCube.transform.RotateAround(movingCube.transform.position, Vector3.up, 350);	
	}

	void OnDragRotate(){
		if(interactiveItem.IsOver){
			Debug.Log("is Over");
			if(movingCube == null && !draggable){
				if(RPCClient.client.Can_Interact(id)){
					draggable = true;
					renderer.material.color = Color.red;
					RPCClient.client.LockGameObject(id);
					movingCube = Instantiate(movingCubeModel);
					movingCube.transform.rotation = this.transform.rotation;
					movingCube.SetActive(true);
				}
			}
		} else if(movingCube != null && draggable){
			draggable = false;
			if(!annotated)
				renderer.material.color = Color.green;
			Vector3 pos = movingCube.transform.position;
			PositionEvent posEvent = new PositionEvent(Device.TANGO, ObjType.CUBE, new Position(pos.x, pos.y, pos.z), id);
			Quaternion dir = movingCube.transform.rotation;
			DirectionEvent dirEvent = new DirectionEvent(Device.TANGO, ObjType.CUBE, new Direction(dir.x, dir.y, dir.z, dir.w), id);
			if(!RPCClient.client.Move_And_Rotate(posEvent, dirEvent))
				Debug.Log("Could not move cube");
			Destroy(movingCube);
			movingCube = null;
		}
	}
	private void OnTeamSpeak(){
		
	}
}
