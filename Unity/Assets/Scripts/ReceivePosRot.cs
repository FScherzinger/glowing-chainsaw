using UnityEngine;
using System;
using System.Collections;
using System.Threading;

using de.dfki.tecs;
using de.dfki.tecs.ps;
using de.dfki.tecs.basetypes;
using System.Runtime.InteropServices;

using de.dfki.events;

public class UpdateHeadScript : MonoBehaviour {

	static PSClient receive_client;
	Thread receive_thread = new Thread( Receive );
	static de.dfki.events.Direction direction;
	static de.dfki.events.Position position;
	static int id = 0;
	static de.dfki.events.Device device;
	public static String serverAddr;
	public static int serverPort = 9000;
	void Start()
	{
		receive_thread.Start();
	}

	void OnApplicationQuit()
	{
		receive_client.Disconnect();
		receive_thread.Abort();
	}

	static void Connect(){
		Debug.Log( "waiting for tecs-server... (receiver)" );
		Uri uri = PSFactory.CreateURI (device + "-" + id, serverAddr, serverPort);
		receive_client = PSFactory.CreatePSClient(uri);
		receive_client.Subscribe( "DirectionEvent" );
		receive_client.Subscribe( "PositionEvent" );
		receive_client.Connect();
		Debug.Log( "connected. (receiver)" );
	}

	static void generateUniqueUri(){
		Uri uri = PSFactory.CreateURI ("Discover", serverAddr, serverPort);
		PSClient client = PSFactory.CreatePSClient(uri);
		client.Connect ();
		long ping = client.Ping (device + "-" + id,"2000");
		if (ping>0){
			id++;
			return(generateUniqueUri());
		}
	}

	static void Receive()
	{

		// start listening
		while( receive_client.IsOnline() )
			while( receive_client.CanRecv() )
			{
				de.dfki.tecs.Event eve = receive_client.Recv();
				switch(eve.Is){
				case "PositionEvent":
					PositionEvent pos_event = eve.ParseData (PositionEvent);
					if (pos_event.id = id && pos_event.Type == device) {
						position = pos_event.Position;
					}


					break;
				case "RotationEvent":
					DirectionEvent dir_event = eve.ParseData (DirectionEvent);
					if (dir_event.id = id && dir_event.Type == device) {
						direction = dir_event.Direction;
					}
					break;
				default:
					return;
				}
					
			}
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
}
