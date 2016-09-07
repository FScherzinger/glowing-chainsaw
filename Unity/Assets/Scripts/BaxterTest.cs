using UnityEngine;
using System.Collections;
using System.Threading;
<<<<<<< HEAD
using de.dfki.tecs.robot.baxter;

=======
using de.dfki.tecs.robot.baxter;

>>>>>>> 67f4d2bfda9eac1182be493ff2e514981a1c81ca
public class BaxterTest : MonoBehaviour {

    Thread thread;

	// Use this for initialization
<<<<<<< HEAD
	void Start () {
        thread = new Thread( new PickAndPlace
        {
            serverAddress = "192.168.1.101",
            serverPort = 9000,
            pap_event = new PickAndPlaceEvent { },
            init = true
        }.Send );

        thread.Start();
=======
	void Start () {
        thread = new Thread( new PickAndPlace
        {
            serverAddress = "192.168.1.101",
            serverPort = 9000,
            pap_event = new PickAndPlaceEvent { },
            init = true
        }.Send );

        thread.Start();
>>>>>>> 67f4d2bfda9eac1182be493ff2e514981a1c81ca
    }
	
	// Update is called once per frame
	void Update () {
	
	}

<<<<<<< HEAD
    void OnApplicationClose()
    {
        thread.Abort();
=======
    void OnApplicationClose()
    {
        thread.Abort();
>>>>>>> 67f4d2bfda9eac1182be493ff2e514981a1c81ca
    }
}
