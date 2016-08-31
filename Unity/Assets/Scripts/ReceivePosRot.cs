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

public class ReceivePosRot : MonoBehaviour {

	static PSClient receive_client;
	Thread receive_thread;


	public GameObject Head;
	public String serveradress = "localhost";
	public int serverport = 9000;
	public Device device;
	public static volatile LinkedList<PositionEvent> PositionEvents;
	public static volatile LinkedList<DirectionEvent> DirectionEvents;
	public volatile Dictionary<int,GameObject> Heads;
	PublishPosRot publishcam;

	void Start()
	{
		publishcam = GameObject.Find("Tango Delta Camera").GetComponent( typeof(PublishPosRot) ) as PublishPosRot;
		PositionEvents = new LinkedList<PositionEvent> ();
		DirectionEvents = new LinkedList<DirectionEvent> ();
		Heads = new Dictionary<int,GameObject>();
		ReceiverThread rc_thread = new ReceiverThread{
			serverAddr = this.serveradress,
			serverPort = this.serverport,
			device = this.device
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
		updateDirection ();
	}

	void updatePosition(){
		if (PositionEvents == null || PositionEvents.Count == 0)
			return;
		PositionEvent pe = PositionEvents.First.Value;
		PositionEvents.RemoveFirst();

		if (pe.Id == publishcam.id)
			return;
		Vector3 newpos = new Vector3((float)pe.Position.X,(float)pe.Position.Y,(float)pe.Position.Z);
		getHead( pe.Id ).transform.position = Vector3.MoveTowards(transform.position,newpos,Time.fixedDeltaTime);
        //Vector3.Lerp (this.transform.position, newpos, Time.deltaTime * 5);

	}



	void updateDirection(){
		if (DirectionEvents == null || DirectionEvents.Count == 0)
			return;
		DirectionEvent de = DirectionEvents.First.Value;
		DirectionEvents.RemoveFirst();
		if (de.Id == publishcam.id)
			return;
		Quaternion newrot = new Quaternion((float)de.Direction.X, (float)de.Direction.Y, (float)de.Direction.Z, (float)de.Direction.W);
        newrot = newrot * this.transform.rotation;
        getHead( de.Id ).transform.rotation = newrot;
        //Quaternion.Lerp(this.transform.rotation, newrot, Time.deltaTime * 30);

	}

	private GameObject getHead(int id){
		if (!Heads.ContainsKey (id)) {
			GameObject head = Instantiate (Head);
			Heads.Add(id,head);
		} 
		return Heads[id];
	}



	public class ReceiverThread {
		public string serverAddr { get; set; }
		public int serverPort { get; set;}
		public Device device { get; set; }

		public void Connect(){
			Debug.Log( "waiting for tecs-server... (receiver)" );
			Uri uri = PSFactory.CreateURI ("receiver_"+device, serverAddr, serverPort);
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
						PositionEvents.AddLast(pos_event);
					}
					if (eve.Is ("DirectionEvent")) {
						DirectionEvent dir_event = new DirectionEvent ();	
						eve.ParseData (dir_event);
						DirectionEvents.AddLast (dir_event);
					}

				}
		}

	}
}
