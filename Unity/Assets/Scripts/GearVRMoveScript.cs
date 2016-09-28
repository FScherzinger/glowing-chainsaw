using UnityEngine;
using System;
using System.Collections;


public class GearVRMoveScript : MonoBehaviour
{
	void Start()
	{
	}

	void Update()
	{
		if( Input.GetKey( KeyCode.F ) )
			transform.Rotate(new Vector3(0,-.1f,0));	
		if( Input.GetKey( KeyCode.H ) )
			transform.Rotate(new Vector3(0,.1f,0));
		if( Input.GetKey( KeyCode.T ) )
			transform.Rotate(new Vector3(-.1f,0,0));	
		if( Input.GetKey( KeyCode.G ) )
			transform.Rotate(new Vector3(.1f,0,0));
	}
}
