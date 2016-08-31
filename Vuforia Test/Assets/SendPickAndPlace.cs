using UnityEngine;
using System.Collections;
using System.Threading;

using de.dfki.tecs.robot.baxter;

public class SendPickAndPlace : MonoBehaviour
{

    private PickAndPlace pick_n_place;
    private Thread thread;

	// Use this for initialization
	void Start()
    {
        pick_n_place = new PickAndPlace
        {
            serverAddr = "localhost",
            serverPort = 9000,
            pap_event = new PickAndPlaceEvent
            {
                Limb = Limb.LEFT,

                Initial_pos = new Position
                {
                    X_left = "1",
                    Y_left = "2",
                    Z_left = "3"
                },

                Final_pos = new Position
                {
                    X_left = "1",
                    Y_left = "2",
                    Z_left = "3"
                },

                Initial_ori = new Orientation
                {
                    Yaw_left = "1",
                    Pitch_left = "1",
                    Roll_left = "1"
                },

                Final_ori = new Orientation
                {
                    Yaw_left = "1",
                    Pitch_left = "1",
                    Roll_left = "1"
                },

                Speed = new Speed
                {
                    Speed_left = "0.2"
                },

                Angls = new Angles(),
                Mode = Reference_sys.ABSOLUTE,
                Kin = Kinematics.INVERSE
            }
        };

        thread = new Thread( pick_n_place.Send );
        thread.Start();
    }

    void OnApplicationClose()
    {
        thread.Abort();
    }
}
