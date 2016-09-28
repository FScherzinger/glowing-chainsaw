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
        GameObject cam = GameObject.Find("Tango AR Camera");

        waittwoframes++;
        if( waittwoframes == 2 )
        {
            gameObject.transform.position = calibrator.transform.position;
            gameObject.transform.rotation = calibrator.transform.rotation;


            Quaternion Qto =calibrator.transform.rotation; // Rotation to match the child to.
            Debug.Log(Qto.eulerAngles);
            Quaternion inv = Quaternion.Inverse(cam.transform.rotation);
            transform.rotation = transform.rotation * (inv * Qto);
        }

        Debug.Log(cam.transform.rotation.eulerAngles);

    }
}
