using System.Threading;
using UnityEngine;
using de.dfki.events;
using System;

public class Client : MonoBehaviour  {


	private Thread receiverThread;
	private Thread rpcThread;
	public int ps_port = 9000;
	public int rpc_port = 9001;
	public string ps_serveraddress = "localhost";
	public string rpc_serveraddress = "localhost";
	public Device device;
	public GameObject objectInitializer;

	RPCClient rpcclient;
	Receiver receiver;

	void Start(){
		//PS
		if (objectInitializer == null)
			Debug.LogError ("Client needs an ObjectInitializer gameobject with attached ObjectInitializer script");
		else {
			ObjectInitializer obj_init = objectInitializer.GetComponent<ObjectInitializer>();
			receiver = new Receiver {
				serverPort = ps_port,
				serverAddr = ps_serveraddress,
				device = device,
				obj_init = obj_init
			};
			receiverThread = new Thread(receiver.Connect);
			receiverThread.Start ();
		}
		//RPC
		rpcclient = new RPCClient {
			address = rpc_serveraddress,
			port = rpc_port		
		};
		rpcThread = new Thread(rpcclient.Connect);
		rpcThread.Start();
	}
		

	void OnApplicationQuit(){
		CloseConnections ();
	}

	void OnDisable(){
		CloseConnections ();
	}
	void CloseConnections(){
		if(receiver !=null )
			receiver.Disconnect ();
		if(receiverThread !=null)
			receiverThread.Join ();
		if(rpcclient != null)
			rpcclient.Disconnect();
		if(rpcThread != null)
			rpcThread.Join();
	}


}
