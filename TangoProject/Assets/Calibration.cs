using UnityEngine;
using System.Collections;

public class Calibration : MonoBehaviour {

	private GameObject calibrator;
    private int waittwoframes=0;

	// Use this for initialization
	void Start(){
		
		calibrator = GameObject.Find("Calibrator");

	}

	void Update(){
        waittwoframes++;
        if( waittwoframes == 2 )
        {
            GameObject cam = GameObject.Find( "Tango AR Camera" );
            Quaternion inverseCam = Quaternion.Inverse( cam.transform.rotation );
            gameObject.transform.rotation = inverseCam * calibrator.transform.rotation;
        }
	}
}
