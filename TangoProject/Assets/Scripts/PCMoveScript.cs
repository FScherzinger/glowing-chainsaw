using UnityEngine;
using System.Collections;

public class PCMoveScript : MonoBehaviour {

	private Vector3 pos;
	const float max_z = 0.4f;
	const float min_z = -2.4f;
	const float max_x = 2.6f;
	const float min_x = -0.2f;
	float y_pos;

	// Use this for initialization
	void Start () {
		y_pos = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		pos = new Vector3();
		if( Input.GetKey( "a" ) )
			pos.x = -.05f;
		if( Input.GetKey( "d" ) )
			pos.x = .05f;
		if( Input.GetKey( "w" ) )
			pos.z = .05f;
		if( Input.GetKey( "s" ) )
			pos.z = -.05f;
		if(pos.x < min_x)
			pos.x = min_x;
		if(pos.x > max_x)
			pos.x = max_x;
		if(pos.z < min_z)
			pos.z = min_z;
		if(pos.z > max_z)
			pos.z = max_z;
		transform.Translate(pos);
		transform.position = new Vector3(transform.position.x, y_pos, transform.position.z);
	}
}
