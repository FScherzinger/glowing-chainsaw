using UnityEngine;
using System.Collections;
using Thrift.Transport;
using Thrift.Server;
using de.dfki.events;
using System;

public class VuforiaServer  {

	// Use this for initialization

	public VuforiaServer(){

		try {
		} catch (Exception x){
			Debug.LogError (x.StackTrace);
		}
		TServerTransport serTransport = new TServerSocket(9000);
		SceneHandler s_handler = new SceneHandler ();
		Scene.Processor proc = new Scene.Processor (s_handler);
		TServer server = new TSimpleServer(proc,serTransport);
		server.Serve ();
		Debug.Log ("Started RPC-Server on Vuforia");

	}
		
}
