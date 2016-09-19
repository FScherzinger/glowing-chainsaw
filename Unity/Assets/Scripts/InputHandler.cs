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
	[SerializeField] private Material normalMaterial;                
	[SerializeField] private Material overMaterial;
	[SerializeField] private Material dragMaterial;
	[SerializeField] private VRInteractiveItem interactiveItem;
	[SerializeField] private Renderer renderer;
	[SerializeField] private GameObject movingCubeModel;
	private GameObject movingCube;
	private int id;

	private bool draggable = false;
		
	private void Awake ()
	{
		renderer.material = normalMaterial;
		id = this.gameObject.GetComponent<MetaData>().id;
	}


	private void OnEnable()
	{
		interactiveItem.OnOver += HandleOver;
		interactiveItem.OnOut += HandleOut;
		interactiveItem.OnClick += Click;
		interactiveItem.OnSwipe += Rotate;
		vRInput.OnClick += VRClick;
	}


	private void OnDisable()
	{
		interactiveItem.OnOver -= HandleOver;
		interactiveItem.OnOut -= HandleOut;
		interactiveItem.OnClick -= Click;
		interactiveItem.OnSwipe -= Rotate;
		vRInput.OnClick -= VRClick;
	}


	//Handle the Over event
	private void HandleOver()
	{
		renderer.material = overMaterial;
	}


	//Handle the Out event
	private void HandleOut()
	{
		if(!draggable)
			renderer.material = normalMaterial;
	}

	//Handle the Click event
	private void Click()
	{
		if(GearVRMenu.currentTool == GearVRMenu.Tool.DRAGNDROP){
			if(movingCube == null && !draggable){
				draggable = true;
				renderer.material = dragMaterial;
				if(RPCClient.client.Can_Interact(id)){
					RPCClient.client.LockGameObject(id);
					movingCube = Instantiate(movingCubeModel);
					movingCube.SetActive(true);
				}
			}
		}else if(GearVRMenu.currentTool == GearVRMenu.Tool.ANNOTATE && interactiveItem.IsOver){
			RPCClient.client.Annotate(new Annotation(id, Device.GEARVR));
		}
	}

	private void VRClick(){
		if(GearVRMenu.currentTool == GearVRMenu.Tool.DRAGNDROP){
			if(!interactiveItem.IsOver){
				if(draggable && movingCube != null){
					draggable = false;
					renderer.material = normalMaterial;
					Vector3 pos = movingCube.transform.position;
					PositionEvent posEvent = new PositionEvent(Device.GEARVR, ObjType.CUBE, new Position(pos.x, pos.y, pos.z), id);
					if(!RPCClient.client.Move(posEvent))
						Debug.Log("Could not move cube");
					Destroy(movingCube);
					movingCube = null;
				}
			}
		}
	}

	private void Rotate(VRInput.SwipeDirection swipeDirection){
		if(GearVRMenu.currentTool == GearVRMenu.Tool.ROTATE){
			float degree;
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
}