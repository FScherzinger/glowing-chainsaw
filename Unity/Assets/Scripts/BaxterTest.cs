using UnityEngine;
using System.Collections;
using System.Threading;
using de.dfki.tecs.robot.baxter;

public class BaxterTest : MonoBehaviour {

    Thread thread;

	// Use this for initialization
	void Start () {
        thread = new Thread( new PickAndPlace
        {
            serverAddress = "192.168.1.101",
            serverPort = 9000,
            pap_event = new PickAndPlaceEvent { },
            init = true
        }.Send );

        thread.Start();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnApplicationClose()
    {
        thread.Abort();
    }
}
