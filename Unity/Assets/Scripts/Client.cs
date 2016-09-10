using System.Threading;
using UnityEngine;
using de.dfki.events;
using System;

public class Client : MonoBehaviour  {


	private System.Threading.Thread receiverThread;
	public int ps_port = 9000;
	public string serveradress = "localhost";
	public Device device;
	public GameObject objectInitializer;


	Receiver receiver;

	void Start(){
		//PS
		ObjectInitializer obj_init = objectInitializer.GetComponent<ObjectInitializer>();
		if (obj_init == null)
			throw new Exception("Client needs an ObjectInitializer gameobject with attached ObjectInitializer script"); 
		receiver = new Receiver {
			serverPort = ps_port,
			serverAddr = serveradress,
			device = device,
			obj_init = obj_init
		};
		receiverThread = new Thread(receiver.Connect);
		receiverThread.Start ();

	}

	void update(){
	}

	void OnApplicationQuit(){
		receiver.Disconnect ();
		receiverThread.Join ();
	}

	void OnDisable(){
		receiver.Disconnect ();
		receiverThread.Join ();
	}


}
