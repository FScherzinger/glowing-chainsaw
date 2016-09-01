
using System;

//libtecs namespace
using de.dfki.tecs.ps;

//Baxter Namespace
using de.dfki.tecs.robot.baxter;


class PickAndPlace
{

    private PSClient client;

    public string serverAddr { get; set; }
    public int serverPort { get; set; }
    public PickAndPlaceEvent pap_event { get; set; }


    private void Connect()
    {
        Uri uri = PSFactory.CreateURI( "csharp-client-pap", serverAddr, serverPort );
        client = PSFactory.CreatePSClient( uri );

        // subscribe before connecting
        client.Subscribe( "DoneEvent" );
        client.Subscribe( "PickAndPlaceEvent" );

        client.Connect();
    }

    public void Send( )
    {
        Connect();

        if( client.IsOnline() && client.IsConnected() )
        {
            // Send event to client "receiver"
            // client.Send([ClientAddress], [Event = Struct Name], [Struct])
            client.Send( "baxter_dummy", "PickAndPlaceEvent", pap_event );
            client.Disconnect();
        }
    }
}