//#define TOUCH

using UnityEngine;
using System.Collections;
using de.dfki.events;

public class CreateDemoCube : MonoBehaviour {

	public GameObject cube;

	public int id ;
	private SceneHandler s_handler;

	// Use this for initialization
	void Start () {
		s_handler = this.gameObject.GetComponent (typeof (SceneHandler)) as SceneHandler;
	}
	
	// Update is called once per frame
	void Update () {


        if( Input.GetKeyDown( "space" ) /*|| Input.GetTouch( 0 ).phase == TouchPhase.Began*/ )

        {

            if (s_handler == null)
				return;
			GameObject clone  = Instantiate(cube);
			id = s_handler.addToSceneObjects (clone);



		}
		if (Input.GetKeyDown ("m")) {
			if (RPCClient.client != null) {
				Position p = new Position (2, 1.1, 0.6);
				PositionEvent pe = new PositionEvent(Device.PC,ObjType.CUBE,p,id);
				RPCClient.client.LockGameObject (id);
				Debug.LogError("MOVE: "+RPCClient.client.Move (pe));
			}
				
		}
	}
}
