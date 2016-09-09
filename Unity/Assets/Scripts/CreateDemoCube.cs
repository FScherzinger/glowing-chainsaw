using UnityEngine;
using System.Collections;

public class CreateDemoCube : MonoBehaviour {

	public GameObject cube;
	private Hashtable cubes;
	public Publisher publisher;

	// Use this for initialization
	void Start () {
		cubes = new Hashtable();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("space")){
			GameObject clone = Instantiate(cube);
			PublishPosRot publisher = clone.GetComponent( typeof(PublishPosRot) ) as PublishPosRot;
			ReceivePosRot.addGameObject(publisher.id,clone);



		}
	}
}
