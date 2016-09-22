using UnityEngine;
using System.Collections;

public class TangoCalibrationScript : MonoBehaviour {

    public GameObject TangoARCamera;
    public GameObject VuforiaARCamera;
    public GameObject FixedMarker;
    private bool calibrating=true;

	// Use this for initialization
	void Start () {
        /*TangoARCamera = GameObject.Find("Tango AR Camera");
        VuforiaARCamera = GameObject.Find("ARCamera");*/
	}
	
	// Update is called once per frame
	void Update () {
        if (calibrating)
        {
            TangoARCamera.transform.position = VuforiaARCamera.transform.position;
            TangoARCamera.transform.rotation = VuforiaARCamera.transform.rotation;
        }
	}

    public void StartCalibration()
    {
        TangoARCamera.SetActive(false);
        VuforiaARCamera.SetActive(true);
        FixedMarker.SetActive(true);
        calibrating = true;
    }

    public void StopCalibration()
    {
        calibrating = false;
        FixedMarker.SetActive(false);
        VuforiaARCamera.SetActive(false);
        TangoARCamera.SetActive(true);
    }
}
