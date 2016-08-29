using UnityEngine;
using System;
using System.Collections;
using System.Threading;

using de.dfki.tecs;
using de.dfki.tecs.ps;
using de.dfki.tecs.basetypes;
using System.Runtime.InteropServices;

using de.dfki.events;

public class ReceivePosRot : MonoBehaviour {

	static PSClient receive_client;
	public de.dfki.events.Device device;
	Thread receive_thread;

	static de.dfki.events.Direction direction;
	static de.dfki.events.Position position;
	static int id = 0;
	public String s;
	public int serverPort = 9000;
	void Start()
	{
		ReceiverThread rc_thread = new ReceiverThread ();
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
		if (position != null) {
			Vector3 newpos = new Vector3((float)position.X, (float)position.Y, (float)position.Z);
			transform.position = Vector3.Lerp (this.transform.position, newpos, Time.deltaTime * 5);
		}
	}

	void updateDirection(){
		if (direction != null) {
			Quaternion newrot = new Quaternion((float)direction.X, (float)direction.Y, (float)direction.Z, (float)direction.W);
			Quaternion upatedrot = Quaternion.Lerp(this.transform.rotation, newrot, Time.deltaTime * 30);
			Vector3 rot_vec = upatedrot.eulerAngles;
			rot_vec.x = 270;
			transform.rotation = Quaternion.Euler (rot_vec);
		}
	}


	public class ReceiverThread {
		string serverAddr { get; set; }
		int serverPort { get; set;}
		Device device { get; set; }

		public void Connect(){
			Debug.Log( "waiting for tecs-server... (receiver)" );
			Uri uri = PSFactory.CreateURI (device + "-" + id, serverAddr, serverPort);
			receive_client = PSFactory.CreatePSClient(uri);
			receive_client.Subscribe( "DirectionEvent" );
			receive_client.Subscribe( "PositionEvent" );
			receive_client.Connect();
			Debug.Log( "connected. (receiver)" );
		}

		void generateUniqueUri(){
			Uri uri = PSFactory.CreateURI ("Discover", serverAddr, serverPort);
			PSClient client = PSFactory.CreatePSClient(uri);
			client.Connect ();
			long ping = client.Ping (device + "-" + id,2000);
			if (ping>0){
				id++;
				generateUniqueUri();
			}
		}

		void Receive(Device device)
		{

			// start listening
			while( receive_client.IsOnline() )
				while( receive_client.CanRecv() )
				{
					de.dfki.tecs.Event eve = receive_client.Recv();
					if (eve.Is ("PositionEvent")) {
						PositionEvent pos_event = new PositionEvent ();
						eve.ParseData (pos_event);
						if (pos_event.Id == id && pos_event.Type == device) {
							position = pos_event.Position;
						}
						continue;
					}
					if (eve.Is ("RotationEvent")) {
						DirectionEvent dir_event = new DirectionEvent ();	
						eve.ParseData (dir_event);
						if (dir_event.Id == id && dir_event.Type == device) {
							direction = dir_event.Direction;
						}
						continue;
					}

				}
		}

	}
}
