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
	public int camID;
	void Start () {
		while (RPCClient.client == null) {
		}
			
		pub = new Publisher {
			serverPort = ps_port,
			serverAddr = ps_server,
			device = device
		};
		pubThread = new Thread(pub.Connect);
		pubThread.Start ();
        camID = RPCClient.client.getUniqueCameraId ();
		publish_pos_rot = PublishPosRot (.2f);
		StartCoroutine (publish_pos_rot);
	}
	

	IEnumerator PublishPosRot(float intervall){
		for (;;) {
			if (pub == null)
				continue;
			else {
				pub.SendPosition (camID,ObjType.CAMERA,this.gameObject);
				pub.SendRotation (camID,ObjType.CAMERA,this.gameObject);
			}
				
			yield return new WaitForSeconds (intervall);
		}
	}
	void OnApplicationQuit(){
		StopCoroutine (publish_pos_rot);
	}
}
