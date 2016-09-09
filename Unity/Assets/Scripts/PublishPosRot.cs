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
    public bool autoUpdate = true;

	public PublishPosRot(){
		System.Random rnd = new System.Random();
		id = rnd.Next();
	}

	void Start()
	{

		ObjType objtype;
		if(this.gameObject.name.ToLower().Contains("cube"))
			objtype = ObjType.CUBE;
		else
			objtype = ObjType.CAMERA;

		publisher = new Publisher {
			device = this.device,
			objname = objtype,
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

	public void sendPosition()
    {
		if(publisher == null){
			return;
		}
			
		publisher.SendPosition( this.gameObject );
		publisher.SendRotation( this.gameObject );
    }

	void OnApplicationQuit(){
		publisher.close ();
		publishThread.Abort ();
	}

}
