using UnityEngine;
using System.Collections;

public class CreateDemoCube : MonoBehaviour {

	public GameObject cube;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("space")){
			Instantiate(cube);
		}
	}
}
