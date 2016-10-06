using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;
using System.Threading;

// This script is a simple example of how an interactive item can
// be used to change things on gameobjects by handling events.
using de.dfki.events;


public class InputHandler : MonoBehaviour
{
	[SerializeField] private VRInput vRInput;
	[SerializeField] private VRInteractiveItem interactiveItem;
	[SerializeField] private Renderer renderer;
	[SerializeField] private GameObject movingCubeModel;
	[SerializeField] private GameObject over;
	private GameObject movingCube;
	private int id;

	private bool annotated = false;
	private bool draggable = false;
		
	public bool isOver(){
		return interactiveItem.IsOver;
	}

	private void Awake ()
	{
		renderer.material.color = Color.green;
		id = this.gameObject.GetComponent<MetaData>().id;
	}


	private void OnEnable()
	{
		interactiveItem.OnOver += HandleOver;
		interactiveItem.OnOut += HandleOut;
		interactiveItem.OnDoubleClick += Click;
		interactiveItem.OnSwipe += Rotate;
		vRInput.OnDoubleClick += VRClick;
		vRInput.OnSwipe += VRSwipe;
	}


	private void OnDisable()
	{
		interactiveItem.OnOver -= HandleOver;
		interactiveItem.OnOut -= HandleOut;
		interactiveItem.OnDoubleClick -= Click;
		interactiveItem.OnSwipe -= Rotate;
		vRInput.OnDoubleClick -= VRClick;
		vRInput.OnSwipe -= VRSwipe;
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

	//Handle the Click event
	private void Click()
	{
		if(GearVRMenu.currentTool == GearVRMenu.Tool.DRAGNDROP && interactiveItem.IsOver){
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
		}else if(GearVRMenu.currentTool == GearVRMenu.Tool.ANNOTATE && interactiveItem.IsOver){
			Annotation an = new Annotation(Device.GEARVR, id);
            if (!RPCClient.client.Annotate(an))
            {
                Debug.Log("Annotation Failed");
            }
		}else if(GearVRMenu.currentTool == GearVRMenu.Tool.DRAGROTATE && interactiveItem.IsOver){
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
		}
	}

	private void VRClick(){
		if(GearVRMenu.currentTool == GearVRMenu.Tool.DRAGNDROP){
			if(!interactiveItem.IsOver){
				if(draggable && movingCube != null){
					draggable = false;
					Vector3 pos = movingCube.transform.position;
					PositionEvent posEvent = new PositionEvent(Device.GEARVR, ObjType.CUBE, new Position(pos.x, pos.y, pos.z), id);
					if(!RPCClient.client.Move(posEvent))
						Debug.Log("Could not move cube");
					Destroy(movingCube);
					movingCube = null;
				}
			}
		}else if(GearVRMenu.currentTool == GearVRMenu.Tool.DRAGROTATE){
			if(!interactiveItem.IsOver){
				if(draggable && movingCube != null){
					draggable = false;
					Vector3 pos = movingCube.transform.position;
					PositionEvent posEvent = new PositionEvent(Device.GEARVR, ObjType.CUBE, new Position(pos.x, pos.y, pos.z), id);
					Quaternion dir = movingCube.transform.rotation;
					DirectionEvent dirEvent = new DirectionEvent(Device.GEARVR, ObjType.CUBE, new Direction(dir.x, dir.y, dir.z, dir.w), id);
					if(!RPCClient.client.Move_And_Rotate(posEvent, dirEvent))
						Debug.Log("Could not move cube");
					Destroy(movingCube);
					movingCube = null;
				}
			}
		}
	}

	private void Rotate(VRInput.SwipeDirection swipeDirection){
		Debug.Log("swipe detected");
		if(GearVRMenu.currentTool == GearVRMenu.Tool.ROTATE){
			switch(swipeDirection){
				case VRInput.SwipeDirection.DOWN:
				case VRInput.SwipeDirection.NONE:
				case VRInput.SwipeDirection.UP:
					return;
				case VRInput.SwipeDirection.LEFT:
					if(RPCClient.client.Can_Interact(id)){
						RPCClient.client.LockGameObject(id);
						movingCube = Instantiate(movingCubeModel);
						movingCube.SetActive(true);
						movingCube.transform.position = this.gameObject.transform.position;
						movingCube.transform.rotation = this.gameObject.transform.rotation;
						this.gameObject.SetActive(false);
						movingCube.transform.RotateAround(movingCube.transform.position, Vector3.down, 10);
					}
					break;
				case VRInput.SwipeDirection.RIGHT:
					if(RPCClient.client.Can_Interact(id)){
						RPCClient.client.LockGameObject(id);
						movingCube = Instantiate(movingCubeModel);
						movingCube.SetActive(true);
						movingCube.transform.position = this.gameObject.transform.position;
						movingCube.transform.rotation = this.gameObject.transform.rotation;
						this.gameObject.SetActive(false);
						movingCube.transform.RotateAround(movingCube.transform.position, Vector3.up, 10);
					}
					break;
			}
			Vector3 pos = this.gameObject.transform.position;
			PositionEvent posEvent = new PositionEvent(Device.GEARVR, ObjType.CUBE, new Position(pos.x, pos.y, pos.z), id);
			Quaternion dir = movingCube.transform.rotation;
			DirectionEvent dirEvent = new DirectionEvent(Device.GEARVR, ObjType.CUBE, new Direction(dir.x, dir.y, dir.z, dir.w), id);
			Destroy(movingCube);
			movingCube = null;
			this.gameObject.SetActive(true);
			if(!RPCClient.client.Move_And_Rotate(posEvent, dirEvent))
				Debug.Log("Could not rotate cube");
		}
	}

	void VRSwipe(VRInput.SwipeDirection swipeDirection){
		if(GearVRMenu.currentTool == GearVRMenu.Tool.DRAGROTATE && draggable && movingCube != null){
			switch(swipeDirection){
			case VRInput.SwipeDirection.DOWN:
			case VRInput.SwipeDirection.NONE:
			case VRInput.SwipeDirection.UP:
				return;
			case VRInput.SwipeDirection.LEFT:
				Debug.Log("dragRotate left swipe");
				movingCube.transform.RotateAround(movingCube.transform.position, Vector3.down, 10);
				break;
			case VRInput.SwipeDirection.RIGHT:
				Debug.Log("dragRotate right swipe");
				movingCube.transform.RotateAround(movingCube.transform.position, Vector3.up, 10);
				break;
			}
		}
	}
}