using UnityEngine;
using System.Collections;
using Thrift.Transport;
using Thrift.Protocol;
using de.dfki.events;

public class RPCClient : MonoBehaviour {

	public static Scene.Client client {get; set;}

	// Use this for initialization
	void Start () {
		TTransport transport = new TSocket("localhost", 9090);
		TProtocol protocol = new TBinaryProtocol(transport);
		client = new Scene.Client(protocol);
	}
}
