﻿using UnityEngine;
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
		if( Input.GetKey( "left" ) )
			transform.Rotate(new Vector3(0,-.1f,0));	
		if( Input.GetKey( "right" ) )
			transform.Rotate(new Vector3(0,.1f,0));
		if( Input.GetKey( "up" ) )
			transform.Rotate(new Vector3(-.1f,0,0));	
		if( Input.GetKey( "down" ) )
			transform.Rotate(new Vector3(.1f,0,0));
	
		/*if( Input.GetKey( "left" ) )
			new_position.x -= .1f;
		if( Input.GetKey( "right" ) )
			new_position.x += .1f;
		if( Input.GetKey( "up" ) )
			new_position.z += .1f;
		if( Input.GetKey( "down" ) )
			new_position.z -= .1f;

		transform.position = new_position;*/
	}
}
