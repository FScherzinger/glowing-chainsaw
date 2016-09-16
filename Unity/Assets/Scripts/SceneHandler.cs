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
    private volatile Dictionary<Position,List<Node>> Nodes; //mapping: position -> list<nodes>

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

            baxterCommunicator.GetComponent<SendPickAndPlace>().SendPAP( curposition, goalpos );

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
            SceneObjects[d.Id].transform.rotation = direction;
        }

        if( ps_publisher == null )
            return;
        foreach( int id in SceneObjects.Keys )
        {
            ps_publisher.SendPosition( id, SceneObjects[id] );
            ps_publisher.SendRotation( id, SceneObjects[id] );
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
        if( SceneObjects.ContainsKey( an.ObjectID ) )
        {

            List<Annotation> obj_annotations;
            //check if an annoation is already assigned to the gameobj
            if( Annotations.ContainsKey( an.ObjectID ) )
            {
                obj_annotations = Annotations[an.ObjectID];
            }
            else
            {
                obj_annotations = new List<Annotation>();
            }
            //push the new annotation
            obj_annotations.Add( an );
            Annotations[an.ObjectID] = obj_annotations;
            return true;
        }
        //return false if gameobject id is invalid
        return false;
    }

    private bool addToNodes( Node node )
    {
        if( SceneObjects.ContainsKey( node.Id ) )
        {
            List<Node> obj_nodes;

            if( Nodes.ContainsKey( node.Position ) )
            {
                obj_nodes = Nodes[node.Position];
            }
            else
            {
                obj_nodes = new List<Node>();
            }
            obj_nodes.Add( node );
            Nodes[node.Position] = obj_nodes;
            return true;
        }
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

    public Information GetInformation( Position pos )
    {
        throw new NotImplementedException();
    }

    public Information GetInformationById( int id )
    {
        throw new System.NotImplementedException();
    }

    public bool Informate( Information info )
    {
        throw new System.NotImplementedException();
    }

    public List<Node> GetNodes( Position _pos )
    {
        foreach( Position pos in Nodes.Keys )
        {
            if( pos == _pos )
                return Nodes[_pos];
        }
        throw new Exception( "No nodes at given position." );
    }


    public Node GetNodeById( int id )
    {
        foreach( Position pos in Nodes.Keys )
        {
            List<Node> nodeList = Nodes[pos];
            foreach( Node node in nodeList )
            {
                if( node.Id == id )
                    return node;
            }
        }
        throw new Exception( "No Node with given id found." );
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

    public bool Node( Node node )
    {
        return addToNodes( node );
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
