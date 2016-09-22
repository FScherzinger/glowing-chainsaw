using System;
using UnityEngine;
using System.Collections.Generic;
using de.dfki.events;
using Thrift.Transport;
using Thrift.Protocol;


/// <summary>
/// Initialize object in the scene and push events to the correct gameobject
/// </summary>
public class ObjectInitializer : MonoBehaviour
{
	/// <summary>
	/// a temporary gameobject which will be instaniated NOW, until a rpc call is implemented for initGameObject to auto detect the prefab based on the id
	/// </summary>
	public GameObject cubeModel;
	public GameObject camModel;
	public PublishCam ownCam;
	public volatile Queue<PositionEvent> pos_events;
	public volatile Queue<DirectionEvent> dir_events;
	private volatile Dictionary<int,ReceivedObject> ObjReceivers; //includes every interactable item, eg. cubes, gameobjects for annotations ...

	void Start(){
		pos_events = new  Queue<PositionEvent> ();
		dir_events = new  Queue<DirectionEvent> ();
		ObjReceivers = new Dictionary<int,ReceivedObject> ();
	}
		

	void Update(){
		if(pos_events != null){
			int pos_counter = pos_events.Count;
			for (int i = 0; i < pos_counter; i++) {
				PositionEvent pe = pos_events.Dequeue ();
				if(pe == null)
					continue;
				ReceivedObject obj = getObjectReceiver (pe.Id, pe.Objtype);
				if(obj != null)
					obj.updatePosition (pe);
			}
		}
		if(dir_events != null){
			int dir_counter = dir_events.Count;
			for (int i = 0; i < dir_counter; i++) {
				DirectionEvent de = dir_events.Dequeue ();
				if(de == null)
					continue;
				ReceivedObject obj = getObjectReceiver (de.Id, de.Objtype);
				if(obj != null)
					obj.updateDirection (de);
			}
		}
	}

	public void handle (PositionEvent p){
		pos_events.Enqueue (p);

	}


	public void handle (DirectionEvent d){
		dir_events.Enqueue (d);

	}

	public ReceivedObject getObjectReceiver(int id, ObjType objTyp){
		if (ObjReceivers == null)
			throw new Exception ("ObjReceivers not yet initialized");
		if (ObjReceivers.ContainsKey (id))
			return ObjReceivers [id];
		else {
			if(id != ownCam.camID){
				GameObject go = initGameObject (id, objTyp);
				if(go != null){
					go.SetActive(true);
					ReceivedObject objr = go.GetComponent<ReceivedObject> ();
					if (objr == null) {
						Debug.LogError("Error instaniated GameObject needs a ObjectReceiver Script attached");
						throw new Exception("Error instaniated GameObject needs a ObjectReceiver Script attached");
					}else {
						ObjReceivers.Add (id, objr);
						return objr;
					}
				}
			}
			return null;
		}
	}

	public GameObject initGameObject(int id, ObjType objType) {
        //TODO: Implement using RPC-Call: Get ObjectType from Server and instantiate correct prefab
		GameObject go = null;
		switch(objType){
			case ObjType.CUBE:
				go = Instantiate(cubeModel);
				if (go.GetComponent<MetaData>() != null) { 
					go.GetComponent<MetaData>().id = id;
					go.GetComponent<MetaData>().ObjType = objType;
				}
				else{
					Debug.LogError("No Meta\tdata attached");
					Debug.Break();
				}
				break;
			case ObjType.CAMERA:
				go = Instantiate(camModel);
				if(go.GetComponent<MetaData>() != null) {
					go.GetComponent<MetaData>().id = id;
					go.GetComponent<MetaData>().ObjType = objType;
				}
				else{
					Debug.LogError("No Meta\tdata attached");
					Debug.Break();
				}
				break;		
		}
		return go;
	}
}


