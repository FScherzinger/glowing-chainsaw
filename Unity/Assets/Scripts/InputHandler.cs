using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;
using System.Threading;

// This script is a simple example of how an interactive item can
// be used to change things on gameobjects by handling events.
public class InputHandler : MonoBehaviour
{
	public Image reticle {get;set;}
	public VRInput m_VRInput;
	[SerializeField] private Material m_NormalMaterial;                
	[SerializeField] private Material m_OverMaterial;                  
	[SerializeField] private Material m_ClickedMaterial;               
	[SerializeField] private Material m_DoubleClickedMaterial;         
	[SerializeField] private VRInteractiveItem m_InteractiveItem;
	[SerializeField] private Renderer m_Renderer;
	private bool draggable = false;


	private enum inputobj{
		VRInput,InteractiveItem
	}

	public void setVRInput(VRInput input){
		m_VRInput = input;
		m_VRInput.OnClick += VRClick;
	}

	private void Awake ()
	{
		m_Renderer.material = m_NormalMaterial;
	}


	private void OnEnable()
	{
		m_InteractiveItem.OnOver += HandleOver;
		m_InteractiveItem.OnOut += HandleOut;
		m_InteractiveItem.OnClick += InteractiveItemClick;
		m_InteractiveItem.OnDoubleClick += HandleDoubleClick;
	}


	private void OnDisable()
	{
		m_InteractiveItem.OnOver -= HandleOver;
		m_InteractiveItem.OnOut -= HandleOut;
		m_InteractiveItem.OnClick -= InteractiveItemClick;
		m_InteractiveItem.OnDoubleClick -= HandleDoubleClick;
		m_VRInput.OnClick -= InteractiveItemClick;
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
	private void InteractiveItemClick()
	{
		m_Renderer.material = m_ClickedMaterial;
		if (!draggable){
			draggable = true;
			m_InteractiveItem.gameObject.GetComponent<Rigidbody>().isKinematic = true;
			m_InteractiveItem.gameObject.GetComponent<BoxCollider>().enabled = false;
		}
	}
	


	//Handle the DoubleClick event
	private void HandleDoubleClick()
	{
		Debug.Log("Show double click");
		m_Renderer.material = m_DoubleClickedMaterial;
	}

	public void Update(){
		if(draggable){
			Vector3 pos = reticle.transform.position;
			pos.y += .1f;
			m_InteractiveItem.transform.position = pos;
		}
	}

	public void VRClick(){
		if (m_InteractiveItem.IsOver)
			return;
		if(draggable){
			draggable = false;

			m_InteractiveItem.gameObject.GetComponent<BoxCollider>().enabled= true;
			m_InteractiveItem.gameObject.GetComponent<Rigidbody>().isKinematic = false;
			//m_publisher.sendPosition();
		}
	}		
}