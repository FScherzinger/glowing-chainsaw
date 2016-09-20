using de.dfki.events;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SceneHandler : MonoBehaviour, Scene.Iface
{

    /// <summary>
    /// How often should the scene be updated in seconds
    /// </summary>

    private volatile List<int> LockedObjects; //includes id of locked gameobjects
    private volatile Dictionary<int,GameObject> SceneObjects; //includes every interactable item, eg. cubes, gameobjects for annotations ...
	private volatile Dictionary<int,List<Annotation>> Annotations; //mapping: gameobject_id -> list<annotation>
	private volatile Dictionary<Position,List<Note>> Notes; //mapping: position -> list<annotation>
	private volatile List<int> note_ids; 
	private volatile List<int> annotation_ids; 

    private volatile Queue<PositionEvent> PositionUpdates;
    private volatile Queue<DirectionEvent> DirectionUpdates;

    private System.Random rnd;
    public Publisher ps_publisher { get; set; }
    public GameObject baxterCommunicator;


    void Start()
    {
        rnd = new System.Random();
        SceneObjects = new Dictionary<int, GameObject>();
        LockedObjects = new List<int>();
        Annotations = new Dictionary<int, List<Annotation>>();
		Notes = new Dictionary<Position, List<Note>>();
        PositionUpdates = new Queue<PositionEvent>();
        DirectionUpdates = new Queue<DirectionEvent>();
		//invoke publishers
		InvokeRepeating("publishPositionRotation", 3, 0.1F);
		InvokeRepeating("publishAnnotationsNotes", 3, 2F);
    }


    void FixedUpdate()
    {
        for( int i = 0; i < PositionUpdates.Count; ++i )
        {
            PositionEvent p = PositionUpdates.Dequeue();

            Vector3 goalpos = new Vector3( (float) p.Position.X,
                        (float) p.Position.Y,
                        (float) p.Position.Z );

            Vector3 curposition = SceneObjects[p.Id].transform.position;
            Vector3 currotation = SceneObjects[p.Id].transform.eulerAngles;

			if(baxterCommunicator!=null)
            	baxterCommunicator.GetComponent<SendPickAndPlace>().SendPAP( curposition, goalpos,currotation,currotation);

            Vector3 position = new Vector3( (float) p.Position.X,
                                            (float) p.Position.Y,
                                            (float) p.Position.Z );
            //SceneObjects[p.Id].transform.position = position;
        }


        for( int i = 0; i < DirectionUpdates.Count; ++i )
        {
            DirectionEvent d = DirectionUpdates.Dequeue();
            Quaternion direction = new Quaternion( (float) d.Direction.X,
                                                   (float) d.Direction.Y,
                                                   (float) d.Direction.Z,
                                                   (float) d.Direction.W );
            Vector3 curposition = SceneObjects[d.Id].transform.position;
            Vector3 currotation = SceneObjects[d.Id].transform.eulerAngles;

			if(baxterCommunicator!=null)
            	baxterCommunicator.GetComponent<SendPickAndPlace>().SendPAP(curposition, curposition, currotation, direction.eulerAngles);
            //SceneObjects[d.Id].transform.rotation = direction;
        }

      

    }

	public void publishPositionRotation(){
		if( ps_publisher == null || PositionUpdates.Count > 0 || DirectionUpdates.Count > 0 )
			return;
		foreach( int id in SceneObjects.Keys )
		{
			//TODO: determinate objtype
			ps_publisher.SendPosition( id, ObjType.CUBE, SceneObjects[id] );
			ps_publisher.SendRotation( id, ObjType.CUBE, SceneObjects[id] );
		}
	}

	public void publishAnnotationsNotes(){
		if( ps_publisher == null )
			return;
		foreach( int objectid in Annotations.Keys )
		{
			foreach (Annotation an in Annotations[objectid])
				ps_publisher.SendAnnotation (an);
		}
		foreach( Position pos in Notes.Keys )
		{
			foreach (Note n in Notes[pos])
				ps_publisher.SendNote (n);
		}
	}

    public int addToSceneObjects( GameObject go )
    {
        if( rnd == null )
            throw new Exception( "Random not yet initialised" );

        int id = rnd.Next();
        while( SceneObjects.ContainsKey( id ) )
            id = rnd.Next();
        SceneObjects.Add( id, go );
        return id;
    }

    private bool addAnnotationToObject( Annotation an )
    {
		if( SceneObjects.ContainsKey( an.ObjectId ) )
		{
			List<Annotation> obj_annotations;
			//check if an annoation is already assigned to the gameobj
			if( Annotations.ContainsKey( an.ObjectId ) )
			{
				obj_annotations = Annotations[an.ObjectId];
			}
			else
			{
				obj_annotations = new List<Annotation>();
			}
			//push the new annotation
			obj_annotations.Add( an );
			Annotations[an.ObjectId] = obj_annotations;
			return true;
		}
        //return false if gameobject id is invalid
        return false;
    }

	private bool addNoteToPosition( Note n ){
		List<Note> pos_notes;
		//normalize position to integers
		Position normalized_pos = NormalizePosition(n.Position);
		if (Notes.ContainsKey (normalized_pos))
			pos_notes = Notes [normalized_pos];
		else
			pos_notes = new List<Note> ();
		//push the new note
		pos_notes.Add(n);
		Notes [normalized_pos] = pos_notes;
		return true;
	}

	private Position NormalizePosition(Position pos){
		double x = Math.Round(pos.X,2);
		double y = Math.Round(pos.Y,2);
		double z = Math.Round(pos.Z,2);
		return new Position (x, y, z);

	}

    #region Iface implementation
    public bool Annotate( Annotation an )
    {
		//generate IDs for new Annotations
		an.Id = rnd.Next ();
		while(annotation_ids.Contains(an.Id))
			an.Id = rnd.Next();
		//push the annotion in the correct list in the dictonary
		if (addAnnotationToObject (an)) {
			annotation_ids.Add (an.Id);
			return true;
		}
		return false;
    }

	public bool Note( Note n )
	{
		//generate IDs for new Notes
		n.Id = rnd.Next ();
		while(note_ids.Contains(n.Id))
			n.Id = rnd.Next();
		//push the note in the correct list in the dictonary
		if (addNoteToPosition (n)) {
			note_ids.Add (n.Id);
			return true;
		}
		return false;
	}



	public bool UpdateAnnotation (int objectId, Annotation an)
	{
		if (!Annotations.ContainsKey (objectId))
			return false;
		//search if list contains an annotation with the id of an
		bool found_an = false;
		foreach (Annotation annotation in Annotations[objectId]){
			if (annotation.Id == an.Id)
				found_an = true;
		}
		if (!found_an)
			return false;
		//delete old annotation
		if (DeleteAnnotation (objectId, an.Id))
			return addAnnotationToObject (an);
		return false;
		
	}
	public bool UpdateNote (Position pos, Note n)
	{
		pos = NormalizePosition(pos);
		n.Position = NormalizePosition (n.Position);
		if (!Notes.ContainsKey (pos))
			return false;
		//search if list contains an note with the id of an
		bool found_note = false;
		foreach (Note note in Notes[pos]){
			if (note.Id == n.Id)
				found_note = true;
		}
		if (!found_note)
			return false;
		//delete old note
		if (DeleteNote (pos, n.Id))
			return addNoteToPosition (n);
		return false;


	}

	public bool DeleteAnnotation (int objectId, int id){
		if(!Annotations.ContainsKey(objectId))
			return false;
		int removes = Annotations [objectId].RemoveAll (x => x.Id == id);
		annotation_ids.Remove (id);
		if (removes == 1)
			return true;
		return false;
	}


	public bool DeleteNote (Position pos, int id){
		pos = NormalizePosition(pos);
		if (!Notes.ContainsKey (pos))
			return false;
		int removes = Notes [pos].RemoveAll (x => x.Id == id);
		note_ids.Remove (id);
		if (removes == 1)
			return true;
		return false;
	}


    public ObjType getObjType( int id )
    {
        if( SceneObjects[id].GetComponent<MetaData>().ObjType == (ObjType.CUBE) )
        {
            return ObjType.CUBE;
        }
        else
        {
            return ObjType.CAMERA;
        }
    }


    /// <summary>
    /// Checks whenever a client can interact with an gameobject
    /// </summary>
    /// <returns><c>true</c>, if gameobject is not in the locked Dictionary, <c>false</c> otherwise.</returns>
    /// <param name="id">Identifier.</param>
    public bool Can_Interact( int id )
    {
        if( LockedObjects.Contains( id ) )
            return false;
        return true;
    }

    public bool LockGameObject( int id )
    {
        if( !Can_Interact( id ) )
        {
            Debug.LogError( "Can not lock the an already locked gameobj" );
            return false;
        }
        if( !SceneObjects.ContainsKey( id ) )
        {
            Debug.LogError( "Can not lock an id which doesn't exist in the scence dictonary" );
            return false;
        }
        LockedObjects.Add( id );
        return true;


    }

    public bool Move( PositionEvent e )
    {
        Debug.Log( "Move entered." );
        //Game Object should be locked before move
        if( Can_Interact( e.Id ) )
        {
            Debug.Log( "Gameobject with id " + e.Id + " should be locked before moving. Aborting..." );
            return false;
        }

        PositionUpdates.Enqueue( e );

        //unlock gameobject
        LockedObjects.Remove( e.Id );
        return true;

    }

    public bool Move_And_Rotate( PositionEvent e, DirectionEvent d )
    {
        if( e.Id != d.Id )
            return false;
        Debug.Log( "Move_And_Rotate entered." );
        // GameObject should be locked vefore move
        if( Can_Interact( e.Id ) )
        {
            Debug.Log( "Gameobject with id " + e.Id + " should be locked before moving. Aborting..." );
            return false;
        }

        PositionUpdates.Enqueue( e );
        DirectionUpdates.Enqueue( d );

        // unlock gameobject
        LockedObjects.Remove( e.Id );
        return true;
    }

    #endregion



}
