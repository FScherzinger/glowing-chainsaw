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
	public GameObject temporaryGO;
	public volatile Queue<PositionEvent> pos_events;
	public volatile Queue<DirectionEvent> dir_events;
	private volatile Dictionary<int,ReceivedObject> ObjReceivers; //includes every interactable item, eg. cubes, gameobjects for annotations ...

	void Start(){
		pos_events = new  Queue<PositionEvent> ();
		dir_events = new  Queue<DirectionEvent> ();
		ObjReceivers = new Dictionary<int,ReceivedObject> ();
	}
		

	void Update(){
		for (int i = 0; i < pos_events.Count; i++) {
			PositionEvent pe = pos_events.Dequeue ();
			ReceivedObject obj = getObjectReceiver (pe.Id);
			obj.updatePosition (pe);

		}
		for (int i = 0; i < dir_events.Count; i++) {
			DirectionEvent de = dir_events.Dequeue ();
			ReceivedObject obj = getObjectReceiver (de.Id);
			obj.updateDirection (de);
		}
	}

	public void handle (PositionEvent p){
		pos_events.Enqueue (p);

	}


	public void handle (DirectionEvent d){
		dir_events.Enqueue (d);

	}

	public ReceivedObject getObjectReceiver(int id){
		if (ObjReceivers == null)
			throw new Exception ("ObjReceivers not yet initialized");

		if (ObjReceivers.ContainsKey (id))
			return ObjReceivers [id];
		else {
			GameObject go = initGameObject (id);
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

	public GameObject initGameObject(int id){
		//TODO: Implement using RPC-Call: Get ObjectType from Server and instantiate correct prefab
		GameObject go = Instantiate(temporaryGO);
		if(go.GetComponent<MetaData>() != null)
			go.GetComponent<MetaData>().id = id;
		else{
			Debug.LogError("No Metadata attached");
			Debug.Break();
		}
		return go;
	}
}


