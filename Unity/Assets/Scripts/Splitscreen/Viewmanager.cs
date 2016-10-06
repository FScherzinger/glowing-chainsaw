using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using de.dfki.events;

public class Viewmanager : MonoBehaviour
{
    // debug variable to change number of views
    public int ViewCount = 1;
    private int max_number_of_camera = 4;
    private int last_count = 0;
    private Camera[] cameras;
    private Camera camPC;
    private Camera camGear;
    private Camera camTango;
    private Camera camVive;

    public void AssignCamPostion(Device device, Vector3 pos){

		switch(device){
		case Device.GEARVR:
			camGear.transform.position = pos;
			break;
		case Device.PC:
			camPC.transform.position = pos;
			break;
		case Device.TANGO:
			camTango.transform.position = pos;
			break;
		case Device.VIVE:
			camVive.transform.position = pos;
			break;
		}
	}

	public void AssignCamDirection(Device device, Quaternion dir){

		switch(device){
		case Device.GEARVR:
			camGear.transform.rotation = dir;
			break;
		case Device.PC:
			camPC.transform.rotation = dir;
			break;
		case Device.TANGO:
			camTango.transform.rotation = dir;
			break;
		case Device.VIVE:
			camVive.transform.rotation = dir;
			break;
		}
	}

	void Start()
    {
        // get all cameras of the current scene
        cameras = Camera.allCameras;

        // label each view to see whitch device we are watching
        foreach( Camera camera in cameras )
            camera.gameObject.GetComponentInChildren<Text>().text = camera.name;

        camPC = GameObject.Find("PC View").GetComponent<Camera>();
        camGear = GameObject.Find("GearVR View").GetComponent<Camera>();
        camTango = GameObject.Find("Tango VIew").GetComponent<Camera>();
        camVive = GameObject.Find("Vive View").GetComponent<Camera>();
    }

    private void UpdateCameras( int count )
    {
        // adjust the cameras viewport Rect
        switch( count )
        {
            case 1:
                // nothing to do here, but still we have to reset every cameraview
                cameras[0].rect = new Rect( 0, 0, 1, 1 );
                cameras[1].rect = new Rect( 0, 0, 0, 0 );
                cameras[2].rect = new Rect( 0, 0, 0, 0 );
                cameras[3].rect = new Rect( 0, 0, 0, 0 );
                break;
            case 2:
                // we vertical split the screen
                cameras[0].rect = new Rect( 0, 0, .5f, 1 );
                cameras[1].rect = new Rect( .5f, 0, .5f, 1 );
                cameras[2].rect = new Rect( 0, 0, 0, 0 );
                cameras[3].rect = new Rect( 0, 0, 0, 0 );
                break;
            case 3:
                // first we split the left half horizontal
                cameras[0].rect = new Rect( 0, 0, .5f, .5f );
                cameras[1].rect = new Rect( 0, .5f, .5f, .5f );
                cameras[2].rect = new Rect( .5f, 0, .5f, 1 );
                cameras[3].rect = new Rect( 0, 0, 0, 0 );
                break;
            case 4:
                // lastly we also split the second half horizontal
                cameras[0].rect = new Rect( 0, 0, .5f, .5f );
                cameras[1].rect = new Rect( 0, .5f, .5f, .5f );
                cameras[2].rect = new Rect( .5f, 0, .5f, .5f );
                cameras[3].rect = new Rect( .5f, .5f, .5f, .5f );
                break;
            default:
                // max four cameras allowed!
                Debug.Log("Max number of cameras is four!");
                return;
        }
        Debug.Log("Cameras Viewport updated.");
    }
	
	void Update()
    {
        // only update the viewport if the debug variable(ViewCount) has changed.
        if( ViewCount > 0 && ViewCount != last_count )
        {
            UpdateCameras( ViewCount );

        }
        last_count = ViewCount;
    }
}
