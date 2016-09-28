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
            GameObject cam = GameObject.Find("Tango AR Camera");
            //Vector3 inverseCam = new Vector3(-cam.transform.eulerAngles.x,
            //    -cam.transform.eulerAngles.y,
            //    -cam.transform.eulerAngles.z);
            //Debug.Log(inverseCam);
            //Vector3 cali = calibrator.transform.eulerAngles;
            //Debug.Log(cali);
            //gameObject.transform.eulerAngles = new Vector3(inverseCam.x,inverseCam.y,inverseCam.z);
            //Debug.Log(gameObject.transform.rotation.eulerAngles);
            gameObject.transform.position = calibrator.transform.position;
            gameObject.transform.rotation = calibrator.transform.rotation;

            Debug.Log(cam.transform.rotation.eulerAngles);

            Quaternion Qto=calibrator.transform.rotation; // Rotation to match the child to.
            Debug.Log(Qto.eulerAngles);
            Quaternion inv = Quaternion.Inverse(cam.transform.rotation);
            Debug.Log(inv.eulerAngles);
            transform.rotation = transform.rotation * (inv * Qto);
            Debug.Log(gameObject.transform.rotation.eulerAngles);

        }
    }
}
