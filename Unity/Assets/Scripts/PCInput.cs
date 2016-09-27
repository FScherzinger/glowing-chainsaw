using UnityEngine;
using System.Collections;
using System;

public class PCInput : MonoBehaviour {

	public event Action OnKeyOne;                            
	public event Action OnKeyTwo;                                 
	public event Action OnWheelUp;
	public event Action OnWheelDown;

	// Update is called once per frame
	void Update () {
		CheckInput();
	}

	private void CheckInput(){
		if (Input.GetKeyDown("1"))
		{
			if (OnKeyOne != null)
				OnKeyOne();
		}
		if (Input.GetKeyDown("2"))
		{
			if (OnKeyTwo != null)
				OnKeyTwo();
		}
		if (Input.GetAxis("Mouse ScrollWheel") != 0){
			if (Input.GetAxis("Mouse ScrollWheel") < 0 && OnWheelDown != null)
				OnWheelDown();
			else if (Input.GetAxis("Mouse ScrollWheel") > 0 && OnWheelUp != null)
				OnWheelUp();
		}
				
	}

	private void OnDestroy()
	{
		// Ensure that all events are unsubscribed when this is destroyed.
		OnKeyOne = null;
		OnKeyTwo = null;
		OnWheelUp = null;
		OnWheelDown = null;
	}
}
