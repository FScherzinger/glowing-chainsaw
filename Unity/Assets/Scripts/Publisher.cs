﻿

using de.dfki.events;
using de.dfki.tecs.ps;
using System;
using UnityEngine;


public class Publisher
{

    PSClient publish_client;

    public Device device { get; set; }
	public ObjType objname { get; set; }
    public string serverAddr { get; set; }
    public int serverPort { get; set; }
    public int id { get; set; }


    public void Connect()
    {
        Debug.Log( "waiting for tecs-server... (publisher)" );
        string connection_id = "p_" + device + "_" + objname+ "_"+ id;
        Uri uri = PSFactory.CreateURI( connection_id, serverAddr, serverPort );
        publish_client = PSFactory.CreatePSClient( uri );
        publish_client.Connect();

        Debug.Log( "conneted as " + connection_id );
    }

    public void SendRotation( GameObject go )
    {
        if( go == null || publish_client == null )
            return;
        if( publish_client.IsOnline() && publish_client.IsConnected() )
        {
			float x = go.transform.rotation.x;
			float y = go.transform.rotation.y;
			float z = go.transform.rotation.z;
			float w = go.transform.rotation.w;
			DirectionEvent de = new DirectionEvent( device,objname, new Direction( x,y,z,w), id );
            publish_client.Send( ".*", "DirectionEvent", de );
        }
    }

    public void SendPosition( GameObject go )
    {
        if( go == null || publish_client == null )
            return;
        if( publish_client.IsOnline() && publish_client.IsConnected() )
        {
			float x = go.transform.position.x;
			float y = go.transform.position.y;
			float z = go.transform.position.z;
		
			PositionEvent pe = new PositionEvent( device,objname, new Position(x,y,z ), id );
            publish_client.Send( ".*", "PositionEvent", pe );
        }
    }

    public void close()
    {
        publish_client.Disconnect();
    }
}
