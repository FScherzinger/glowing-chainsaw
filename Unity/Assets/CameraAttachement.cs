using UnityEngine;
using System.Collections;

public class CameraAttachement : MonoBehaviour {

	[SerializeField] public GameObject ARCam;
	
	// Update is called once per frame
	void Update () {
		transform.position = ARCam.transform.position;
	}
}
