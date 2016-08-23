using UnityEngine;
using System;
using System.Collections;
using System.Threading;

using de.dfki.tecs;
using de.dfki.tecs.ps;
using de.dfki.tecs.basetypes;
using System.Runtime.InteropServices;

using de.dfki.events;

public class PublishCamScript : MonoBehaviour {

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
		Uri uri = PSFactory.CreateURI( "publish-gear-client", "192.168.43.105", 9000 );
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
		if( publish_client.IsOnline() && publish_client.IsConnected() )
		{
			Direction direction = new Direction( transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w );
			DirectionEvent de = new DirectionEvent( MsgType.GEARVR, direction );
			publish_client.Send( ".*", "DirectionEvent", de );
		}
	}
}
