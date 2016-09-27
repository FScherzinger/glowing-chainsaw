using UnityEngine;
using System.Collections;

public class Calibration : MonoBehaviour {

	private GameObject calibrator;

	// Use this for initialization
	void Start(){
		
		calibrator = GameObject.Find("Calibrator");
		gameObject.transform.position = calibrator.transform.position;
		GameObject cam = GameObject.Find("Tango AR Camera");
		Quaternion inverseCam = Quaternion.Inverse(cam.transform.rotation);
		gameObject.transform.rotation = inverseCam * calibrator.transform.rotation;
	}

	void Update(){
		//cam.transform.position = calibrator.transform.position;
		//cam.transform.rotation = calibrator.transform.rotation;
	}
}
