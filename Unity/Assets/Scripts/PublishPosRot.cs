using UnityEngine;
using System;
using System.Collections;
using System.Threading;

using de.dfki.events;

public class PublishPosRot : MonoBehaviour {

	Publisher publisher;
	Thread publishThread;
	public Device device;
	public String serverAddr = "localhost";
	public int serverPort = 9000;
	public int id { get; set; }
    public bool autoUpdate = false;

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
        if( autoUpdate )
        {
            publisher.SendPosition( gameObject );
            publisher.SendRotation( gameObject );
        }
	}

    public void sendPosition(GameObject go )
    {
        publisher.SendPosition( go );
        publisher.SendRotation( go );
    }

	void OnApplicationQuit(){
		publisher.close ();
		publishThread.Abort ();
	}

}
