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
    public GameObject BaxterObject;
	public String serveradress = "localhost";
	public int serverport = 9000;
	public Device device;
	public static volatile Queue<PositionEvent> PositionEvents;
	public static volatile Queue<DirectionEvent> DirectionEvents;
	public volatile Dictionary<int,GameObject> Objects;
    public GameObject PublisherObject;
	PublishPosRot publishcam;

	void Start()
	{
		publishcam = PublisherObject.GetComponent( typeof(PublishPosRot) ) as PublishPosRot;
		PositionEvents = new Queue<PositionEvent> ();
		DirectionEvents = new Queue<DirectionEvent> ();
		Objects = new Dictionary<int,GameObject>();
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
		if (PositionEvents.Count == 0)
			return;
		PositionEvent pe = PositionEvents.Dequeue ();
		if (pe.Id == publishcam.id)
			return;
		Vector3 newpos = new Vector3((float)pe.Position.X,(float)pe.Position.Y,(float)pe.Position.Z);
        if (pe.Type == Device.BAXTER)
        {
            getBaxterObject(pe.Id).transform.position = newpos;
        }
        else
        {
            if ((this.transform.position - newpos).sqrMagnitude > 0.00001)
                getHead(pe.Id).transform.position = newpos;//Vector3.Lerp (this.transform.position, newpos, Time.fixedDeltaTime);
        }

	}



	void updateDirection(){
		if (DirectionEvents.Count == 0)
			return;
		DirectionEvent de =DirectionEvents.Dequeue();
		if (de.Id == publishcam.id)
			return;

		Quaternion newrot = new Quaternion((float)de.Direction.X, (float)de.Direction.Y, (float)de.Direction.Z, (float)de.Direction.W);
        newrot = newrot * this.transform.rotation;
        if (de.Type == Device.BAXTER)
        {
            getBaxterObject(de.Id).transform.rotation = newrot;
        }else
            getHead( de.Id ).transform.rotation = newrot;
        //Quaternion.Lerp(this.transform.rotation, newrot, Time.deltaTime * 30);

	}

	private GameObject getHead(int id){
		if (!Objects.ContainsKey (id)) {
			GameObject head = Instantiate (Head);
			Objects.Add(id,head);
		} 
		return Objects[id];
	}

    private GameObject getBaxterObject(int id)
    {
        if (!Objects.ContainsKey(id))
        {
            GameObject obj = Instantiate(BaxterObject);
            Objects.Add(id, obj);
        }
        return Objects[id];
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
