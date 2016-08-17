using UnityEngine;
using System;
using System.Collections;
using System.Threading;

using de.dfki.tecs;
using de.dfki.tecs.ps;
using de.dfki.tecs.basetypes;

using de.dfki.test;

public class PublisherScript : MonoBehaviour {

	static PSClient publish_client;
	Thread connection_thread = new Thread( Connect );
	static bool is_connected = false;

	void Start()
	{
		connection_thread.Start();
	}

	static void Connect()
	{
		Debug.Log( "waiting for tecs-server... (publisher)" );
		Uri uri = PSFactory.CreateURI( "publish-client", "localhost", 9000 );
		publish_client = PSFactory.CreatePSClient( uri );
		publish_client.Connect();
		is_connected = true;
		Debug.Log( "connected. (publisher)" );
	}

	void OnApplicationQuit()
	{
		publish_client.Disconnect();
		connection_thread.Abort();
	}

	void Update()
	{
		Publish();
	}

	void Publish()
	{
		if( !is_connected )
			return;
		TestEvent te = new TestEvent( transform.position.x, transform.position.y, transform.position.z );
		publish_client.Send( ".*", "TestEvent", te );
	}
}
