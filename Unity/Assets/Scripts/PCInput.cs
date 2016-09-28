using UnityEngine;
using System.Collections;
using System;

public class PCInput : MonoBehaviour {

	public event Action OnKeyOne;                            
	public event Action OnKeyTwo;
	public event Action OnKeyThree;  
	public event Action OnKeyFour;
	public event Action OnWheelUp;
	public event Action OnWheelDown;

	// Update is called once per frame
	void Update () {
		CheckInput();
	}

	private void CheckInput(){
		if (Input.GetKeyDown("1"))
		{
			Debug.Log("1 pressed");
			if (OnKeyOne != null)
				OnKeyOne();
		}
		if (Input.GetKeyDown("2"))
		{
			Debug.Log("2 pressed");
			if (OnKeyTwo != null)
				OnKeyTwo();
		}
		if (Input.GetKeyDown("3"))
		{
			Debug.Log("3 pressed");
			if (OnKeyThree != null)
				OnKeyThree();
		}
		if (Input.GetKeyDown("4"))
		{
			Debug.Log("4 pressed");
			if (OnKeyFour != null)
				OnKeyFour();
		}
		if (Input.GetAxis("Mouse ScrollWheel") != 0){
			Debug.Log("scrolled mouse wheel");
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
