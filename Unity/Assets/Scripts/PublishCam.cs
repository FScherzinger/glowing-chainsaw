using UnityEngine;
using System.Collections;
using System.Threading;
using de.dfki.events;

public class PublishCam : MonoBehaviour {

	// Use this for initialization
	public string ps_server;
	public int ps_port;
	public Device device;
	private Publisher pub ;
	private Thread pubThread;
	private IEnumerator publish_pos_rot;
	public int camID = 0;
    public RPCClient rpclient;
	void Start () {
	
		pub = new Publisher {
			serverPort = ps_port,
			serverAddr = ps_server,
			device = device
		};
		pubThread = new Thread(pub.Connect);
		pubThread.Start ();
      
		publish_pos_rot = PublishPosRot (.2f);
		StartCoroutine (publish_pos_rot);
	}
	

	IEnumerator PublishPosRot(float intervall){
		for (;;) {
            if( pub == null )
                continue;
            if (camID == 0 )
                camID = RPCClient.client.getUniqueCameraId();
			else {
				pub.SendPosition (camID,ObjType.CAMERA,this.gameObject);
				switch(device){
				case Device.GEARVR:
					pub.SendRotation (camID,ObjType.CAMERA,this.gameObject.GetComponent<Camera>());
					break;
				case Device.TANGO:
					break;
				case Device.VIVE:
				case Device.PC:
					pub.SendRotation (camID,ObjType.CAMERA,this.gameObject);
					break;
				}
			}
				
			yield return new WaitForSeconds (intervall);
		}
	}
	void OnApplicationQuit(){
		StopCoroutine (publish_pos_rot);
	}
}
