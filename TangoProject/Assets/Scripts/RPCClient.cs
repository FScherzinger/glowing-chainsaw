﻿using UnityEngine;
using System.Collections;
using Thrift.Transport;
using Thrift.Protocol;
using de.dfki.events;
using System;

public class RPCClient {

	public static Scene.Client client {get; set;}
	public string address {get;set;}
	public int port {get;set;}

   public  static bool connected = false;
	  TTransport transport;


	// Use this for initialization
	public void Connect () {
		try {
			transport = new TSocket(address, port);
			TProtocol protocol = new TBinaryProtocol(transport);
			client = new Scene.Client(protocol);
			transport.Open();
            connected = true;
			Debug.Log("connected");
		} catch (Exception e){
			Debug.LogError(e);
		}
	}



	public void Disconnect(){
		if(transport.IsOpen)
			transport.Close();
	}
}
