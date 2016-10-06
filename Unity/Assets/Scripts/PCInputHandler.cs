using UnityEngine;
using System.Collections;
using VRStandardAssets.Utils;
using de.dfki.events;

public class PCInputHandler : MonoBehaviour {

	[SerializeField] private PCInput PcInput;
	[SerializeField] private Renderer renderer;
	[SerializeField] private GameObject over;
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
		interactiveItem.OnOver += HandleOver;
		interactiveItem.OnOut += HandleOut;
		PcInput.OnKeyOne += OnDragDrop;
		PcInput.OnKeyTwo += OnDragRotate;
		PcInput.OnKeyThree += OnAnnotate;
		PcInput.OnWheelUp += OnRotateLeft;
		PcInput.OnWheelDown += OnRotateRight;
	}

	private void OnDisable()
	{
		interactiveItem.OnOver -= HandleOver;
		interactiveItem.OnOut -= HandleOut;
		PcInput.OnKeyOne -= OnDragDrop;
		PcInput.OnKeyTwo -= OnAnnotate;
		PcInput.OnWheelUp -= OnRotateLeft;
		PcInput.OnWheelDown -= OnRotateRight;
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
		if(!draggable && !annotated)
			renderer.material.color = Color.green;
		over.SetActive(false);
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
			PositionEvent posEvent = new PositionEvent(Device.PC, ObjType.CUBE, new Position(pos.x, pos.y, pos.z), id);
			if(!RPCClient.client.Move(posEvent))
				Debug.Log("Could not move cube");
			Destroy(movingCube);
			movingCube = null;
		}
	}

	void OnAnnotate(){
		if(interactiveItem.IsOver){
			Annotation an = new Annotation(Device.PC, id);
			if (!RPCClient.client.Annotate(an))
			{
				Debug.Log("Annotation Failed");
			}
		}
	}

	void OnRotateLeft(){
		if(movingCube != null && draggable)
			movingCube.transform.RotateAround(movingCube.transform.position, Vector3.down, 10);
		/*else if(interactiveItem.IsOver){
			if(RPCClient.client.LockGameObject(id)){
				movingCube = Instantiate(movingCubeModel);
				movingCube.SetActive(true);
				movingCube.transform.position = this.gameObject.transform.position;
				movingCube.transform.rotation = this.gameObject.transform.rotation;
				this.gameObject.SetActive(false);
				movingCube.transform.RotateAround(movingCube.transform.position, Vector3.down, 10);
				Vector3 pos = this.gameObject.transform.position;
				PositionEvent posEvent = new PositionEvent(Device.PC, ObjType.CUBE, new Position(pos.x, pos.y, pos.z), id);
				Quaternion dir = movingCube.transform.rotation;
				DirectionEvent dirEvent = new DirectionEvent(Device.PC, ObjType.CUBE, new Direction(dir.x, dir.y, dir.z, dir.w), id);
				Destroy(movingCube);
				movingCube = null;
				this.gameObject.SetActive(true);
				if(!RPCClient.client.Move_And_Rotate(posEvent, dirEvent))
					Debug.Log("Could not rotate cube");
			}
		}*/
	}

	void OnRotateRight(){
		if(movingCube != null && draggable)
			movingCube.transform.RotateAround(movingCube.transform.position, Vector3.up, 10);
		/*else if(interactiveItem.IsOver){
			if(RPCClient.client.LockGameObject(id)){
				movingCube = Instantiate(movingCubeModel);
				movingCube.SetActive(true);
				movingCube.transform.position = this.gameObject.transform.position;
				movingCube.transform.rotation = this.gameObject.transform.rotation;
				this.gameObject.SetActive(false);
				movingCube.transform.RotateAround(movingCube.transform.position, Vector3.up, 10);
				Vector3 pos = this.gameObject.transform.position;
				PositionEvent posEvent = new PositionEvent(Device.PC, ObjType.CUBE, new Position(pos.x, pos.y, pos.z), id);
				Quaternion dir = movingCube.transform.rotation;
				DirectionEvent dirEvent = new DirectionEvent(Device.PC, ObjType.CUBE, new Direction(dir.x, dir.y, dir.z, dir.w), id);
				Destroy(movingCube);
				movingCube = null;
				this.gameObject.SetActive(true);
				if(!RPCClient.client.Move_And_Rotate(posEvent, dirEvent))
					Debug.Log("Could not rotate cube");
			}
		}*/
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
			PositionEvent posEvent = new PositionEvent(Device.PC, ObjType.CUBE, new Position(pos.x, pos.y, pos.z), id);
			Quaternion dir = movingCube.transform.rotation;
			DirectionEvent dirEvent = new DirectionEvent(Device.PC, ObjType.CUBE, new Direction(dir.x, dir.y, dir.z, dir.w), id);
			if(!RPCClient.client.Move_And_Rotate(posEvent, dirEvent))
				Debug.Log("Could not move cube");
			Destroy(movingCube);
			movingCube = null;
		}
	}
}