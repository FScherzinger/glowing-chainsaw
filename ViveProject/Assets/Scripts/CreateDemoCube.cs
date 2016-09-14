﻿using UnityEngine;
using System.Collections;

public class CreateDemoCube : MonoBehaviour {

	public GameObject cube;
	private Hashtable cubes;
	private SceneHandler s_handler;

	// Use this for initialization
	void Start () {
		cubes = new Hashtable();
		s_handler = this.gameObject.GetComponent (typeof (SceneHandler)) as SceneHandler;
	}
	
	// Update is called once per frame
	void Update () {
        if( Input.touchCount < 0 )
            return;
        if(Input.GetKeyDown("space") || Input.GetTouch(0).phase == TouchPhase.Began){

			if (s_handler == null)
				return;
			GameObject clone = Instantiate(cube);
			s_handler.addToSceneObjects (clone);



		}
	}
}
