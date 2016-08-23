using UnityEngine;
using System.Collections;
using System.Threading;

using de.dfki.tecs;
using de.dfki.tecs.ps;
using de.dfki.tecs.basetypes;
using System.Runtime.InteropServices;
using de.dfki.events;
using System;

public class PublishCamScript : MonoBehaviour {

	static PSClient publish_client;
	Thread connection_thread = new Thread( Connect );
	static bool is_connected = false;

	// Use this for initialization
	void Start () {
		connection_thread.Start();

	}

	void OnApplicationQuit()
	{
		publish_client.Disconnect();
		connection_thread.Abort();
	}


	static void Connect()
	{
		Debug.Log( "waiting for tecs-server... (publisher)" );
		Uri uri = PSFactory.CreateURI( "publish-tangoClient", "192.168.43.105", 9000 );
		publish_client = PSFactory.CreatePSClient( uri );
		publish_client.Connect();
		is_connected = true;
		Debug.Log( "connected. (publisher)" );
	}

	void Publish()
	{
		if( !is_connected )
			return;
		if( publish_client.IsOnline() && publish_client.IsConnected() )
		{
			Direction dir = new Direction( transform.rotation.x, transform.rotation.y, transform.rotation.z,transform.rotation.w );
			DirectionEvent de = new DirectionEvent( MsgType.TANGO, dir );
			publish_client.Send( ".*", "DirectionEvent", de );
		}
	}

	// Update is called once per frame
	void Update () {
		Publish();
	}
}
