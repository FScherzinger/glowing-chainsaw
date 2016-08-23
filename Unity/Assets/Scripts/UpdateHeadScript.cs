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

	void Start()
	{
		receive_thread.Start();
	}

	void OnApplicationQuit()
	{
		receive_client.Disconnect();
		receive_thread.Abort();
	}

	static void Receive()
	{
		// establish connection
		Debug.Log( "waiting for tecs-server... (receiver)" );
		Uri uri = PSFactory.CreateURI( "update-head-tango-client", "192.168.0.13", 9000 );
		receive_client = PSFactory.CreatePSClient( uri );
		receive_client.Subscribe( "DirectionEvent" );
		receive_client.Connect();
		Debug.Log( "connected. (receiver)" );
		// start listening
		while( receive_client.IsOnline() )
			while( receive_client.CanRecv() )
			{
				de.dfki.tecs.Event eve = receive_client.Recv();
				if( eve.Is( "DirectionEvent" ) )
				{
					DirectionEvent de = new DirectionEvent();
					eve.ParseData(de);
					switch(de.Type){
//						case MsgType.TANGO:
//							direction = de.Direction;
//							break;

						case MsgType.GEARVR:
							direction = de.Direction;
							break;

//						case MsgType.VIVE:
//							direction = de.Direction;
//							break;
						default:
							break;
					}
				}
			}
	}

	void Update(){
		if (direction != null) {
			Quaternion newrot = new Quaternion((float)direction.X, (float)direction.Y, (float)direction.Z, (float)direction.W);
			Quaternion upatedrot = Quaternion.Lerp(this.transform.rotation, newrot, Time.deltaTime * 30);
			Vector3 rot_vec = upatedrot.eulerAngles;
			rot_vec.x = 270;
			transform.rotation = Quaternion.Euler (rot_vec);
		}

	}
}
