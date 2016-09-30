using System;
using de.dfki.events;
using System.Collections.Generic;
using de.dfki.tecs.ps;
using UnityEngine;

	public class Receiver {
		public string serverAddr { get; set; }
		public int serverPort { get; set;}
		public Device device { get; set; }
		public ObjectInitializer obj_init { get; set; }

		
		private PSClient receive_client;

		public void Connect(){
			Debug.Log( "waiting for tecs-server... (receiver)" );
			Uri uri = PSFactory.CreateURI ("r_" + device , serverAddr, serverPort);
			receive_client = PSFactory.CreatePSClient(uri);
			receive_client.Subscribe( "DirectionEvent" );
			receive_client.Subscribe( "PositionEvent" );
            receive_client.Subscribe("Annotation");
            receive_client.Connect();
			Debug.Log( "connected. (receiver)" );
			Receive ();
		}

		void Receive()
		{

			// start listening
			while( receive_client.IsOnline() ){
				while( receive_client.CanRecv() )
				{
					if (obj_init == null)
						continue;
						
					de.dfki.tecs.Event eve = receive_client.Recv();

					if (eve.Is ("PositionEvent")) {
						PositionEvent pos_event = new PositionEvent ();
						eve.ParseData (pos_event);
						obj_init.handle (pos_event);
					}
					if (eve.Is ("DirectionEvent")) {
						DirectionEvent dir_event = new DirectionEvent ();	
						eve.ParseData (dir_event);
						obj_init.handle (dir_event);
					}
					if(eve.Is ("Annotation")){
						Annotation an = new Annotation();
						eve.ParseData(an);
						obj_init.handle(an);
					}
				}
			}
	}


	public void Disconnect(){
		if(receive_client.IsOnline())
			receive_client.Disconnect ();
	}
}



