using de.dfki.events;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SceneHandler : MonoBehaviour, Scene.Iface {

	private volatile List<int> LockedObjects; //includes id of locked gameobjects
	private volatile Dictionary<int,GameObject> SceneObjects; //includes every interactable item, eg. cubes, gameobjects for annotations ...
	private volatile Dictionary<int,List<Annotation>> Annotations; //mapping: gameobject_id -> list<annotation>
	private volatile Dictionary<Position,List<Annotation>> Nodes; //mapping: position -> list<nodes>
	private System.Random rnd;

	public SceneHandler(){
		SceneObjects = new Dictionary<int,GameObject> ();
		LockedObjects = new List<int>();
		Annotations = new  Dictionary<int,List<Annotation>> ();
	}

	void Start (){
		rnd = new System.Random();
		GameObject[] gos = UnityEngine.Object.FindObjectsOfType<GameObject> (); //managed objects
		foreach (GameObject go in gos) {
			//TODO: Filter relevant gameobjects
				addToSceneObjects (go);
		}
	}

	void Update(){
	}

	public int addToSceneObjects(GameObject go){

		int id = rnd.Next ();
		while (SceneObjects.ContainsKey (id))
			id = rnd.Next();
		SceneObjects.Add (id, go);
		return id;
	}

	public bool addToAnnotations(Annotation an){
		if (SceneObjects.ContainsKey (an.ObjectID)) {
			
			List<Annotation> obj_annotations;
			//check if an annoation is already assigned to the gameobj
			if (Annotations.ContainsKey (an.ObjectID)) {
				obj_annotations = Annotations [an.ObjectID];
			} else {
				obj_annotations = new List<Annotation> ();
			}
			//push the new annotation
			obj_annotations.Add (an);
			Annotations [an.ObjectID] = obj_annotations;
			return true;
		}
		//return false if gameobject id is invalid
		return false;
	}


	#region Iface implementation
	public bool Annotate (Annotation an)
	{
		return addToAnnotations (an);
	}
		
	public List<Annotation> GetAnnotations (int objectId)
	{
		if (Annotations.ContainsKey) {
			if (Annotations [objectId]) {
				List<Annotation> obj_annotations = Annotations [objectId];
				return obj_annotations;
			}
			throw new Exception("Gameobject has no annotations attached");
		}
		throw new Exception("The Gameobject id is invalid");
	

	}

	public Annotation GetAnnotationById (int id)
	{
		throw new System.NotImplementedException ();
	}

	public Information GetInformation (Position pos)
	{
		throw new NotImplementedException ();
	}

	public Information GetInformationById (int id)
	{
		throw new System.NotImplementedException ();
	}

	public bool Informate (Information info)
	{
		throw new System.NotImplementedException ();
	}
		
	public List<Node> GetNodes (Position pos)
	{
		throw new System.NotImplementedException ();
	}


	public Node GetNodeById (int id)
	{
		throw new System.NotImplementedException ();
	}

	public bool Node (Node node)
	{
		throw new System.NotImplementedException ();
	}
	/// <summary>
	/// Checks whenever a client can interact with an gameobject
	/// </summary>
	/// <returns><c>true</c>, if gameobject is not in the locked Dictionary, <c>false</c> otherwise.</returns>
	/// <param name="id">Identifier.</param>
	public bool Can_Interact(int id){
		if (LockedObjects.Contains(id))
			return false;
		return true;
	}

	public bool LockGameObject (int id){
		if (!Can_Interact (id)) {
			Debug.LogError("Can not lock the an already locked gameobj");
			return false;
		}
		if (!SceneObjects.ContainsKey (id)) {
			Debug.LogError("Can not lock an id which doesn't exist in the scence dictonary");
			return false;
		}
		LockedObjects.Add (id);
		return true;
		
		
	}

	public bool Move (PositionEvent e)
	{
		//Game Object should be locked before move
		if (Can_Interact (e.Id)) {
			Debug.Log("Gameobject with id "+e.Id+" should be locked before moving. Aborting...");
			return false;
		}
			
		GameObject go = SceneObjects [e.Id];
		if (go == null)
			return false;
		
		Vector3 new_pos = new Vector3 ((float)e.Position.X, (float)e.Position.Y,(float) e.Position.Z);
		go.transform.position = new_pos;
		//unlock gameobject
		LockedObjects.Remove(e.Id);
		return true;

	}

	public bool Move_And_Rotate (PositionEvent e, DirectionEvent d)
	{
		throw new System.NotImplementedException ();
	}

	#endregion



}
