using UnityEngine;
using System;
using System.Collections;
using System.Threading;

using de.dfki.tecs;
using de.dfki.tecs.ps;
using de.dfki.tecs.basetypes;

using de.dfki.test;

public class ReceiverScript : MonoBehaviour {

	static PSClient client;
	Thread thread = new Thread(Receive);

	void Start () {
		Uri uri = PSFactory.CreateURI("receiver-client", "localhost", 9000);
		client = PSFactory.CreatePSClient(uri);
		client.Subscribe("TestEvent");
		client.Connect();
		thread.Start();
	}

	void OnApplicationClose(){
		client.Disconnect();
		thread.Abort();
	}

	static void Receive () {
		while(client.IsOnline())
			while(client.CanRecv()){
				de.dfki.tecs.Event eve = client.Recv();
				if(eve.Is("TestEvent")){
					TestEvent te = new TestEvent();
					eve.ParseData(te);
					Debug.Log(te.X + " , " + te.Y + " , " + te.Z);
				}
			}
	}
}
