using UnityEngine;
using UnityEngine.SceneManagement;
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
		transform.position = new Vector3(1,0,0);
	}

	// Update is called once per frame
	void Update () {
		//transform.position = VuforiaARCamera.transform.position;
		//transform.rotation = VuforiaARCamera.transform.rotation;
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

	public void switchScene(){
		SceneManager.LoadScene("TangoScene");
	}
}
