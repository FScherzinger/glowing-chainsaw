﻿using UnityEngine;
using System.Collections;
using VRStandardAssets.Utils;
using de.dfki.events;

public class PCInputHandler : MonoBehaviour {

	[SerializeField] private PCInput PcInput;
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
		PcInput.OnKeyOne += OnDragDrop;
		PcInput.OnKeyTwo += OnAnnotate;
		PcInput.OnWheelUp += OnRotateLeft;
		PcInput.OnWheelDown += OnRotateRight;
	}

	private void OnDisable()
	{
		PcInput.OnKeyOne -= OnDragDrop;
		PcInput.OnKeyTwo -= OnAnnotate;
		PcInput.OnWheelUp -= OnRotateLeft;
		PcInput.OnWheelDown -= OnRotateRight;
	}
	
	void OnDragDrop(){
		if(interactiveItem.IsOver){
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
	}

	void OnRotateRight(){
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
	}
}