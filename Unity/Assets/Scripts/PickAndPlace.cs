
using System;

using de.dfki.tecs.ps;
using de.dfki.tecs.robot.baxter;
using de.dfki.tecs;

class PickAndPlace
{

    private PSClient client;

    public string serverAddress { get; set; }
    public int serverPort { get; set; }
    public PickAndPlaceEvent pap_event { get; set; }
    public bool init { get; set; }

    private bool waiting_for_baxter = true;

    /* standard position from where we will start and where we will end after
     * executing a PickAndPlaceEvent */
    private MoveArmEvent standardPosition = new MoveArmEvent
    {
        Limb = Limb.RIGHT,
        //TODO(itchyy): change the position to a proper one.
        Pos = new Position
        {
            X_right = "0.66098",
            Y_right = "-0.20035",
            Z_right = "0.35652",
        },
        Ori = new Orientation
        {
            Yaw_right = "-0.11008",
            Pitch_right = "0.03708",
            Roll_right = "-3.11924",
        },
        Speed = new Speed
        {
            Speed_right = "0.2",
        },
        Angls = new Angles(),
        Mode = Reference_sys.ABSOLUTE,
        Kin = Kinematics.INVERSE
    };

    private void Connect()
    {
        Uri uri = PSFactory.CreateURI( "csharp-client-pap", serverAddress, serverPort );
        client = PSFactory.CreatePSClient( uri );

//TODO(itchyy): implement receiving and processing DoneEvent.
        client.Subscribe( "DoneEvent" );
        client.Subscribe( "PickAndPlaceEvent" );
        client.Subscribe( "MoveArmEvent" );

        client.Connect();
    }

    public void Send()
    {
        Connect();

        if( client.IsOnline() && client.IsConnected() )
        {
            // before moving to first object always move arm to initial position.
            client.Send( "receiver_right", "MoveArmEvent", standardPosition );
            waiting_for_baxter = true;
            // wait for DoneEvent before continuing.
            while( waiting_for_baxter ||client.CanRecv() )
            {
                Event evt = client.Recv();
                if( evt.Is( "DoneEvent" ) )
                {
                    waiting_for_baxter = false;
                    DoneEvent done_event = new DoneEvent();
                    evt.ParseData( done_event );

                    //TODO(itchyy): properly handle error.
                    if( done_event.Error )
                    {
                        // we received an error, aborting...
                    }
                    else
                    {
                        // MoveEvent succeeded, going on...
                        if( !init )
                        {
                            client.Send( "receiver_right", "PickAndPlaceEvent", pap_event );
                        }
                    }
                    break;
                }
            }
            // after placing the object to the new position we move the arm back to its initial position.
            if( !init )
                client.Send( "receiver_right", "MoveArmEvent", standardPosition );

            client.Disconnect();
        }
    }
}
