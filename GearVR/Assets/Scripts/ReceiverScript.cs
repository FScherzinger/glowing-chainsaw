using UnityEngine;
using System;
using System.Collections;
using System.Threading;

using de.dfki.tecs;
using de.dfki.tecs.ps;
using de.dfki.tecs.basetypes;
using System.Runtime.InteropServices;

using de.dfki.test;

public class ReceiverScript : MonoBehaviour {

	static PSClient receive_client;
	Thread receive_thread = new Thread( Receive );
	static Vector3 vec;

	void Start()
	{
		vec = new Vector3(0,0,0);
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
		Uri uri = PSFactory.CreateURI( "receiver-client", "192.168.1.141", 9000 );
		receive_client = PSFactory.CreatePSClient( uri );
		receive_client.Subscribe( "TestEvent" );
		receive_client.Connect();
		Debug.Log( "connected. (receiver)" );

		// start listening
		while( receive_client.IsOnline() )
			while( receive_client.CanRecv() )
			{
				de.dfki.tecs.Event eve = receive_client.Recv();
				if( eve.Is( "TestEvent" ) )
				{
					TestEvent te = new TestEvent();
					eve.ParseData(te);
					vec = new Vector3((float)te.X, (float)te.Y, (float)te.Z) ;
				}
			}
	}

	void Update(){
		transform.position = vec;
	}
}
