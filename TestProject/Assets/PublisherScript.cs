using UnityEngine;
using System;
using System.Collections;

using de.dfki.tecs;
using de.dfki.tecs.ps;
using de.dfki.tecs.basetypes;

using de.dfki.test;

public class PublisherScript : MonoBehaviour {

	PSClient client;

	void Start () {
		Uri uri = PSFactory.CreateURI("publish-client", "localhost", 9000);
		client = PSFactory.CreatePSClient(uri);
		client.Connect();
	}

	void Update(){
		Publish ();
	}
	
	void Publish(){
		TestEvent te = new TestEvent(transform.position.x, transform.position.y, transform.position.z);
		client.Send(".*", "TestEvent", te);
	}
}
