using UnityEngine;
using System;
using System.Collections;

using de.dfki.tecs;
using de.dfki.tecs.ps;
using de.dfki.tecs.basetypes;

using de.dfki.test;

public class MoveScript : MonoBehaviour {

	PSClient client;
		
	void Update () {
		if(Input.GetKey("left"))
			transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z);
		else if(Input.GetKey("right"))
			transform.position = new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z);
		else if(Input.GetKey("up"))
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f);
		else if(Input.GetKey("down"))
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f);
	}

}
