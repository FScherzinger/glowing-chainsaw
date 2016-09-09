using System;
using System.Collections;
using Thrift.Transport;
using Thrift.Server;
using de.dfki.events;
using UnityEngine;

public class RPCServer
{
	TServer server;
	TServerTransport serTransport;
	Scene.Processor proc;
	public RPCServer(SceneHandler s_handler,int port){
		serTransport = new TServerSocket(port);
		proc = new Scene.Processor (s_handler);	
		
	}

	public void startServer(){
		try {
			server = new TSimpleServer(proc,serTransport);
			server.Serve();
			Debug.Log ("Started RPC-Server on Vuforia");
		} catch (Exception x){
			Debug.LogError (x.StackTrace);
		}
	}

	public void stopServer(){
		server.Stop ();
		Debug.Log ("Stoped RPC-Server on Vuforia");
	}

}

