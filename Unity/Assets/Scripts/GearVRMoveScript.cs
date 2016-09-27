using UnityEngine;
using System;
using System.Collections;


public class MoveScript : MonoBehaviour
{
	private Vector3 new_position;
	[SerializeField] private Canvas menu;

	void Start()
	{
		new_position = transform.position;
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
