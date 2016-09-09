using UnityEngine;
using System;
using System.Collections;
using System.Threading;

using de.dfki.tecs;
using de.dfki.tecs.ps;
using de.dfki.tecs.basetypes;
using System.Runtime.InteropServices;

using de.dfki.events;
using System.Collections.Generic;
using VRStandardAssets.Utils;
using UnityEngine.UI;

public class ReceivePosRot : MonoBehaviour {

	static PSClient receive_client;
	Thread receive_thread;


    public GameObject renderObject;
	public String serveradress = "localhost";
	public int serverport = 9000;
	public Device device;
	public static volatile Queue<PositionEvent> PositionEvents;
	public static volatile Queue<DirectionEvent> DirectionEvents;
	private volatile Dictionary<int,GameObject> Objects;
	public GameObject publisherObject;
	private PublishPosRot publisher;


    void Start()
	{
		if (publisherObject != null )
			publisher = publisherObject.GetComponent( typeof(PublishPosRot) ) as PublishPosRot;
		PositionEvents = new Queue<PositionEvent> ();
		DirectionEvents = new Queue<DirectionEvent> ();
		Objects = new Dictionary<int,GameObject>();
		ObjType objtype;
		if(this.gameObject.name.ToLower().Contains("cube"))
			objtype = ObjType.CUBE;
		else
			objtype = ObjType.CAMERA;
		ReceiverThread rc_thread = new ReceiverThread{
			serverAddr = this.serveradress,
			serverPort = this.serverport,
			device = this.device,
			objtype = objtype
		};
		receive_thread = new Thread (rc_thread.Connect);
		receive_thread.Start();

	}

	void OnApplicationQuit()
	{
		receive_client.Disconnect();
		receive_thread.Abort();
	}


	void Update(){
		updatePosition ();
		//updateDirection ();
	}

	void updatePosition(){
		if (PositionEvents.Count == 0)
			return;
		for(int i = 0 ; i<PositionEvents.Count;i++){
			PositionEvent pe = PositionEvents.Dequeue ();
			if (publisherObject != null && pe.Id == publisher.id)
				continue;
			Vector3 newpos = new Vector3((float)pe.Position.X,(float)pe.Position.Y,(float)pe.Position.Z);
		//	if ( (transform.position - newpos).sqrMagnitude > 0.00001){
				Debug.Log(transform.position);
				GameObject head = getObject(pe.Id);
				head.transform.position = newpos;
				// newpos;//Vector3.Lerp (this.transform.position, newpos, Time.fixedDeltaTime);
		//	}
		}
	}



	void updateDirection(){
		if (DirectionEvents.Count == 0)
			return;

		for(int i =0;i<DirectionEvents.Count;i++){

			DirectionEvent de =DirectionEvents.Dequeue();
			if (publisherObject !=null && de.Id == publisher.id)
				continue;

			Quaternion newrot = new Quaternion((float)de.Direction.X, (float)de.Direction.Y, (float)de.Direction.Z, (float)de.Direction.W);
			newrot = newrot * this.transform.rotation;
			getObject( de.Id ).transform.rotation = newrot;
			//Quaternion.Lerp(this.transform.rotation, newrot, Time.deltaTime * 30);
		}


	}

	private GameObject getObject(int id){
		if (!Objects.ContainsKey (id)) {
			GameObject obj = Instantiate (renderObject);
			//Cube
			initCube(obj);
			Objects.Add(id,obj);
			//head.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
		} 
		return Objects[id];
	}

	public void initCube(GameObject cube){
		InputHandler handler = cube.GetComponent( typeof(InputHandler)) as InputHandler;
		handler.reticle = (Image)GameObject.Find("Reticle").GetComponent<Image>();
		VRInput input = GameObject.Find("Main Camera").GetComponent<VRInput>();
		handler.setVRInput(input);
	}

    public class ReceiverThread {
		public string serverAddr { get; set; }
		public int serverPort { get; set;}
		public Device device { get; set; }
		public ObjType objtype { get; set; }

		public void Connect(){
			Debug.Log( "waiting for tecs-server... (receiver)" );
			Uri uri = PSFactory.CreateURI ("r_" + device + "_" + objtype, serverAddr, serverPort);
			receive_client = PSFactory.CreatePSClient(uri);
			receive_client.Subscribe( "DirectionEvent" );
			receive_client.Subscribe( "PositionEvent" );
			receive_client.Connect();
			Debug.Log( "connected. (receiver)" );
			Receive ();
		}

		void Receive()
		{

			// start listening
			while( receive_client.IsOnline() )
				while( receive_client.CanRecv() )
				{
					de.dfki.tecs.Event eve = receive_client.Recv();
					if (eve.Is ("PositionEvent")) {
						PositionEvent pos_event = new PositionEvent ();
						eve.ParseData (pos_event);
						PositionEvents.Enqueue(pos_event);
					}
					if (eve.Is ("DirectionEvent")) {
						DirectionEvent dir_event = new DirectionEvent ();	
						eve.ParseData (dir_event);
						DirectionEvents.Enqueue (dir_event);
					}

				}
		}
	}
}
