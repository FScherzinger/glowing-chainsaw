using UnityEngine;
using System;
using System.Collections;
using System.Threading;

using de.dfki.tecs;
using de.dfki.tecs.ps;
using de.dfki.tecs.basetypes;
using System.Runtime.InteropServices;

using de.dfki.events;

public class ReceiverScript : MonoBehaviour {

	static PSClient receive_client;
	Thread receive_thread = new Thread( Receive );
	static Position position;

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
		Uri uri = PSFactory.CreateURI( "receiver-client-vive", "192.168.1.141", 9000 );
		receive_client = PSFactory.CreatePSClient( uri );
		receive_client.Subscribe( "PositionEvent" );
		receive_client.Connect();
		Debug.Log( "connected. (receiver)" );

		// start listening
		while( receive_client.IsOnline() )
			while( receive_client.CanRecv() )
			{
				de.dfki.tecs.Event eve = receive_client.Recv();
				if( eve.Is( "PositionEvent" ) )
				{
					PositionEvent pe = new PositionEvent();
					eve.ParseData(pe);
					switch(pe.Type){
					case MsgType.TANGO:
						position = pe.Position ;
						break;
					
					case MsgType.GEARVR:
						position = pe.Position ;
						break;

					case MsgType.VIVE:
						position = pe.Position ;
						break;
					default:
						break;
					}
				}
			}
	}

	void Update(){
	//	transform.position = new Vector3((float)position.X, (float)position.Y, (float)position.Z);
	}
}
