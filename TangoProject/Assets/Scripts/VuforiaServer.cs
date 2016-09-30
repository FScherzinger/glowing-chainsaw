using System.Threading;
using UnityEngine;
using de.dfki.events;

public class VuforiaServer : MonoBehaviour  {


	private System.Threading.Thread rpcServerThread;
	private System.Threading.Thread vuforiaPublisherThread;
	public int rpc_port = 9001;
	public int ps_port = 9000;
	public string serveradress = "localhost";


	RPCServer rpcServer;
	Publisher vuforiaPublisher;

	void Start(){
		//RPC
		SceneHandler s_handler = this.gameObject.GetComponent (typeof (SceneHandler)) as SceneHandler;
		rpcServer = new RPCServer (s_handler,rpc_port);
		rpcServerThread = new Thread(rpcServer.startServer);
		rpcServerThread.Start ();
		//PS
		vuforiaPublisher = new Publisher {
			serverPort = ps_port,
			serverAddr = serveradress,
			device = Device.VUFORIA
		};
		vuforiaPublisherThread = new Thread(vuforiaPublisher.Connect);
		vuforiaPublisherThread.Start ();
		s_handler.ps_publisher = vuforiaPublisher;
	
	}

	void update(){
	}

	void OnApplicationQuit(){
		rpcServer.stopServer ();
		rpcServerThread.Join ();
		vuforiaPublisher.Disconnect ();
		vuforiaPublisherThread.Join ();
	}

	void OnDisable(){
		rpcServer.stopServer ();
		rpcServerThread.Join ();
		vuforiaPublisher.Disconnect ();
		vuforiaPublisherThread.Join ();
	
	}

		
}
