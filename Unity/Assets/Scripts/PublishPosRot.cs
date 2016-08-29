using UnityEngine;
using System;
using System.Collections;
using System.Threading;

using de.dfki.tecs;
using de.dfki.tecs.ps;
using de.dfki.tecs.basetypes;

using de.dfki.events;

public class PublishPosRot : MonoBehaviour {

	Publisher publisher;
	static bool is_connected = false;
	public Device device;

	void Start()
	{
		publisher = new Publisher ();
		Thread publishThread = new Thread( publisher.Connect );
	}



	void OnApplicationQuit()
	{
		publisher.close ();
	}

	void FixedUpdate(){

		publisher.SendPosition (gameObject);
		publisher.SendRotation (gameObject);
		
	}
		




	public class Publisher{
		PSClient publish_client;
		public Device device { get; set; }
		public string serverAddr { get; set; }
		public int serverPort { get; set;}
		int id = 0;
		public void Connect()
		{
			Debug.Log( "waiting for tecs-server... (publisher)" );
			generateUniqueUri ();
			Uri uri = PSFactory.CreateURI("publisher_"+device + "-" + id, serverAddr, serverPort );
			publish_client = PSFactory.CreatePSClient( uri );
			publish_client.Connect();
			Debug.Log( "connected. (publisher)" );
		}


		void generateUniqueUri(){
			Uri uri = PSFactory.CreateURI ("Discover", serverAddr, serverPort);
			PSClient client = PSFactory.CreatePSClient(uri);
			client.Connect ();
			long ping = client.Ping ("publisher_"+device + "-" + id,2000);
			if (ping>0){
				id++;
				generateUniqueUri();
			}
			Debug.Log ("Deveice:"+device.ToString()+" | id:"+id);
		}



		public void SendRotation(GameObject go){
			if( !is_connected )
				return;
			
			if( publish_client.IsOnline() && publish_client.IsConnected() )
			{
				DirectionEvent de = new DirectionEvent( device, new Direction(go.transform.rotation),id);
				publish_client.Send( ".*", "DirectionEvent", de );
			}
		}

		public void SendPosition(GameObject go)
		{
			if( !is_connected )
				return;
			if( publish_client.IsOnline() && publish_client.IsConnected() )
			{
				PositionEvent pe = new PositionEvent(device, new Position(go.transform.position),id);
				publish_client.Send( ".*", "PositionEvent", pe );
			}
		}

		public void close(){
			publish_client.Disconnect();

		}
	}
}
