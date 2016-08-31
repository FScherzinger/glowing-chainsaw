using UnityEngine;
using System.Collections;
using System.Threading;

using de.dfki.tecs.robot.baxter;

public class SendEvent : MonoBehaviour {

    PickAndPlace pap;
    Thread thread;

	// Use this for initialization
	void Start () {

        // create event
        PickAndPlaceEvent pape = createPaPEvent();

        pap = new PickAndPlace
        {
            serverAddr = "localhost",
            serverPort = 9000,
            pap = pape
        };

        thread = new Thread( pap.Send );
        thread.Start();
    }

    private PickAndPlaceEvent createPaPEvent()
    {
        PickAndPlaceEvent pape = new PickAndPlaceEvent();
        pape.Limb = Limb.LEFT;

        pape.Initial_pos = new Position();
        pape.Initial_pos.X_left = "1";
        pape.Initial_pos.Y_left = "2";
        pape.Initial_pos.Z_left = "3";

        pape.Final_pos = new Position();
        pape.Final_pos.X_left = "1";
        pape.Final_pos.Y_left = "2";
        pape.Final_pos.Z_left = "3";

        pape.Initial_ori = new Orientation();
        pape.Initial_ori.Yaw_left = "1";
        pape.Initial_ori.Pitch_left = "1";
        pape.Initial_ori.Roll_left = "1";

        pape.Final_ori = new Orientation();
        pape.Final_ori.Yaw_left = "1";
        pape.Final_ori.Pitch_left = "1";
        pape.Final_ori.Roll_left = "1";

        pape.Speed = new Speed();
        pape.Speed.Speed_left = "0.2";

        pape.Angls = new Angles();
        pape.Mode = Reference_sys.ABSOLUTE;
        pape.Kin = Kinematics.INVERSE;

        return pape;
    }

    void OnApplicationClose()
    {
        thread.Abort();
    }
}
