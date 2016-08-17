using UnityEngine;
using System;
using System.Collections;
using System.Threading;

using de.dfki.tecs;
using de.dfki.tecs.ps;
using de.dfki.tecs.basetypes;

using de.dfki.test;

public class ReceiverScript : MonoBehaviour {

	static PSClient receive_client;
	Thread receive_thread = new Thread( Receive );

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
		Uri uri = PSFactory.CreateURI( "receiver-client", "localhost", 9000 );
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
					Debug.Log( te.X + " , " + te.Y + " , " + te.Z );
				}
			}
	}
}
