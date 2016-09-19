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
        PositionUpdates = new Queue<PositionEvent>();
        DirectionUpdates = new Queue<DirectionEvent>();
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

            baxterCommunicator.GetComponent<SendPickAndPlace>().SendPAP(curposition, curposition, currotation, direction.eulerAngles);
            //SceneObjects[d.Id].transform.rotation = direction;
        }

        if( ps_publisher == null )
            return;
        foreach( int id in SceneObjects.Keys )
        {
			//TODO: determinate objtype
			ps_publisher.SendPosition( id, ObjType.CUBE, SceneObjects[id] );
			ps_publisher.SendRotation( id, ObjType.CUBE, SceneObjects[id] );
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

    private bool addToAnnotations( Annotation an )
    {

		if (an.ObjectId > 0) {
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
		}
		if (an.Position != null)
			throw new System.NotImplementedException ();

        //return false if gameobject id is invalid
        return false;
    }


    #region Iface implementation
    public bool Annotate( Annotation an )
    {
        return addToAnnotations( an );
    }

    public List<Annotation> GetAnnotations( int objectId )
    {
        if( Annotations.ContainsKey( objectId ) )
        {
            if( Annotations[objectId] != null )
            {
                List<Annotation> obj_annotations = Annotations[objectId];
                return obj_annotations;
            }
            throw new Exception( "Gameobject has no annotations attached" );
        }
        throw new Exception( "The Gameobject id is invalid" );


    }

    public Annotation GetAnnotationById( int _id )
    {
        foreach( int id in Annotations.Keys )
        {
            List<Annotation> annotationList = Annotations[id];
            foreach( Annotation anno in annotationList )
            {
                if( anno.Id == _id )
                    return anno;
            }
        }
        throw new Exception( "No annotation with given id found." );
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
