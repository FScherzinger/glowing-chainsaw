using UnityEngine;
using System.Collections;
using VRStandardAssets.Utils;
using de.dfki.events;

public class TangoInputHandler : MonoBehaviour {

	[SerializeField] private TangoInput tangoInput;
	[SerializeField] private Material normalMaterial;                
	[SerializeField] private Material overMaterial;
	[SerializeField] private Material dragMaterial;
	[SerializeField] private Material annotationMaterial;
	[SerializeField] private VRInteractiveItem interactiveItem;
	[SerializeField] private GameObject movingCubeModel;
	private GameObject movingCube;
	private int id;

	private bool annotated = false;
	private bool draggable = false;

	void Awake () {
		id = this.gameObject.GetComponent<MetaData>().id;
	}

	private void OnEnable()
	{
		tangoInput.OnSelect += OnSelect;
		tangoInput.OnRotateLeft += OnRotateLeft;
		tangoInput.OnRotateRight += OnRotateRight;
	}

	private void OnDisable()
	{
		tangoInput.OnSelect -= OnSelect;
		tangoInput.OnRotateLeft -= OnRotateLeft;
		tangoInput.OnRotateRight -= OnRotateRight;
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
					gameObject.GetComponent<Renderer>().material = dragMaterial;
					RPCClient.client.LockGameObject(id);
					movingCube = Instantiate(movingCubeModel);
					movingCube.SetActive(true);
				}
			}
		} else if(movingCube != null && draggable){
			draggable = false;
			if(!annotated)
				gameObject.GetComponent<Renderer>().material = normalMaterial;
			else
				gameObject.GetComponent<Renderer>().material = annotationMaterial;
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
		}
	}

	void OnRotateLeft(){
		if(movingCube != null && draggable)
			movingCube.transform.RotateAround(movingCube.transform.position, Vector3.down, 10);
		else if(interactiveItem.IsOver){
			if(RPCClient.client.LockGameObject(id)){
				movingCube = Instantiate(movingCubeModel);
				movingCube.SetActive(true);
				movingCube.transform.position = this.gameObject.transform.position;
				movingCube.transform.rotation = this.gameObject.transform.rotation;
				this.gameObject.SetActive(false);
				movingCube.transform.RotateAround(movingCube.transform.position, Vector3.down, 10);
				Vector3 pos = this.gameObject.transform.position;
				PositionEvent posEvent = new PositionEvent(Device.TANGO, ObjType.CUBE, new Position(pos.x, pos.y, pos.z), id);
				Quaternion dir = movingCube.transform.rotation;
				DirectionEvent dirEvent = new DirectionEvent(Device.TANGO, ObjType.CUBE, new Direction(dir.x, dir.y, dir.z, dir.w), id);
				Destroy(movingCube);
				movingCube = null;
				this.gameObject.SetActive(true);
				if(!RPCClient.client.Move_And_Rotate(posEvent, dirEvent))
					Debug.Log("Could not rotate cube");
			}
		}
	}

	void OnRotateRight(){
		if(movingCube != null && draggable)
			movingCube.transform.RotateAround(movingCube.transform.position, Vector3.up, 10);
		else if(interactiveItem.IsOver){
			if(RPCClient.client.LockGameObject(id)){
				movingCube = Instantiate(movingCubeModel);
				movingCube.SetActive(true);
				movingCube.transform.position = this.gameObject.transform.position;
				movingCube.transform.rotation = this.gameObject.transform.rotation;
				this.gameObject.SetActive(false);
				movingCube.transform.RotateAround(movingCube.transform.position, Vector3.up, 10);
				Vector3 pos = this.gameObject.transform.position;
				PositionEvent posEvent = new PositionEvent(Device.TANGO, ObjType.CUBE, new Position(pos.x, pos.y, pos.z), id);
				Quaternion dir = movingCube.transform.rotation;
				DirectionEvent dirEvent = new DirectionEvent(Device.TANGO, ObjType.CUBE, new Direction(dir.x, dir.y, dir.z, dir.w), id);
				Destroy(movingCube);
				movingCube = null;
				this.gameObject.SetActive(true);
				if(!RPCClient.client.Move_And_Rotate(posEvent, dirEvent))
					Debug.Log("Could not rotate cube");
			}
		}
	}

	void OnDragRotate(){
		if(interactiveItem.IsOver){
			Debug.Log("is Over");
			if(movingCube == null && !draggable){
				if(RPCClient.client.Can_Interact(id)){
					draggable = true;
					gameObject.GetComponent<Renderer>().material = dragMaterial;
					RPCClient.client.LockGameObject(id);
					movingCube = Instantiate(movingCubeModel);
					movingCube.SetActive(true);
				}
			}
		} else if(movingCube != null && draggable){
			draggable = false;
			if(!annotated)
				gameObject.GetComponent<Renderer>().material = normalMaterial;
			else
				gameObject.GetComponent<Renderer>().material = annotationMaterial;
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
