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
	Thread publishThread;
	public Device device;
	public String serverAddr = "localhost";
	public int serverPort = 9000;
	public int id { get; set; }

	void Start()
	{
		System.Random rnd = new System.Random();
		id = rnd.Next();
		publisher = new Publisher {
			device = this.device,
			serverAddr = this.serverAddr,
			serverPort = this.serverPort,
			id = this.id
		};
		publishThread = new Thread( publisher.Connect );
		publishThread.Start ();
	}




	void FixedUpdate(){

		publisher.SendPosition (gameObject);
		publisher.SendRotation (gameObject);
		
	}

	void OnApplicationQuit(){
		publisher.close ();
		publishThread.Abort ();
	}
		




	public class Publisher{

		PSClient publish_client;
	
		public Device device { get; set; }
		public string serverAddr { get; set; }
		public int serverPort { get; set;}
		public int id { get; set;}


		public void Connect()
		{
			Debug.Log( "waiting for tecs-server... (publisher)" );
			string connection_id = "publisher_" + device + "_" + id;
			Uri uri = PSFactory.CreateURI(connection_id, serverAddr, serverPort );
			publish_client = PSFactory.CreatePSClient( uri );
			publish_client.Connect();

			Debug.Log ("conneted as "+connection_id);
		}




		public void SendRotation(GameObject go){
			if (go==null)
				return;
			if( publish_client.IsOnline() && publish_client.IsConnected() )
			{
				DirectionEvent de = new DirectionEvent( device, new Direction(go.transform.rotation),id);
				publish_client.Send( ".*", "DirectionEvent", de );
			}
		}

		public void SendPosition(GameObject go){
			if (go==null)
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
