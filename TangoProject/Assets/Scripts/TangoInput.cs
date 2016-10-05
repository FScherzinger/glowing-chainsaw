using UnityEngine;
using System.Collections;
using System;

public class TangoInput : MonoBehaviour {

	public event Action OnSelect;                            
	public event Action OnRotateLeft;
	public event Action OnRotateRight;  

	public void Select(){
		Debug.Log("Select pressed");
		if (OnSelect != null)
			OnSelect();
	}

	public void RotateRight(){
		Debug.Log("Right pressed");
		if (OnRotateRight != null)
			OnRotateRight();
	}

	public void RotateLeft(){
		Debug.Log("Left pressed");
		if (OnRotateLeft!= null)
			OnRotateLeft();
	}

	private void OnDestroy()
	{
		// Ensure that all events are unsubscribed when this is destroyed.
		OnSelect = null;
		OnRotateLeft = null;
		OnRotateRight = null;
	}
}
