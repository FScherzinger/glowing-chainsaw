using UnityEngine;
using System.Collections;

public class Calibration : MonoBehaviour {

	private GameObject calibrator;

	// Use this for initialization
	void Start(){
		
		calibrator = GameObject.Find("Calibrator");
		gameObject.transform.position = calibrator.transform.position;
		gameObject.transform.rotation = calibrator.transform.rotation;
	}

	void Update(){
		//cam.transform.position = calibrator.transform.position;
		//cam.transform.rotation = calibrator.transform.rotation;
	}
}
