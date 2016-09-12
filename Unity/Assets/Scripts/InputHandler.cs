using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;
using System.Threading;

// This script is a simple example of how an interactive item can
// be used to change things on gameobjects by handling events.
using de.dfki.events;


public class InputHandler : MonoBehaviour
{
	[SerializeField] private VRInput m_VRInput;
	[SerializeField] private Material m_NormalMaterial;                
	[SerializeField] private Material m_OverMaterial;                  
	[SerializeField] private VRInteractiveItem m_InteractiveItem;
	[SerializeField] private Renderer m_Renderer;
	[SerializeField] private GameObject m_MovingCubeModel;
	private GameObject m_MovingCube;

	private bool draggable = false;
		
	private void Awake ()
	{
		m_Renderer.material = m_NormalMaterial;
	}


	private void OnEnable()
	{
		m_InteractiveItem.OnOver += HandleOver;
		m_InteractiveItem.OnOut += HandleOut;
		m_InteractiveItem.OnClick += Click;
		m_VRInput.OnClick += VRClick;
	}


	private void OnDisable()
	{
		m_InteractiveItem.OnOver -= HandleOver;
		m_InteractiveItem.OnOut -= HandleOut;
		m_InteractiveItem.OnClick -= Click;
		m_VRInput.OnClick -= VRClick;
	}


	//Handle the Over event
	private void HandleOver()
	{
		Debug.Log("Show over state");
		m_Renderer.material = m_OverMaterial;
	}


	//Handle the Out event
	private void HandleOut()
	{
		Debug.Log("Show out state");
		m_Renderer.material = m_NormalMaterial;
	}

	//Handle the Click event
	private void Click()
	{
		Debug.Log("Click fired");
		int id = this.gameObject.GetComponent<MetaData>().id;
		if(m_MovingCube == null && !draggable){
			draggable = true;
			if(RPCClient.client.Can_Interact(id)){
				RPCClient.client.LockGameObject(id);
				m_MovingCube = Instantiate(m_MovingCubeModel);
			}
		}
	}

	private void VRClick(){
		Debug.Log("VRClick fired");
		int id = this.gameObject.GetComponent<MetaData>().id;
		if(draggable && m_MovingCube != null){
			draggable = false;
			Vector3 pos = m_MovingCube.transform.position;
			PositionEvent posEvent = new PositionEvent(Device.GEARVR, ObjType.CUBE, new Position(pos.x, pos.y, pos.z), id);
			//RPCClient.client.Move(posEvent);
			Destroy(m_MovingCube);
			m_MovingCube = null;
		}
	}
}