using UnityEngine;
using System.Collections;

public class MoveScript : MonoBehaviour {

	Vector3 vec = new Vector3(1,0,0);
	int i = 0;
	// Update is called once per frame
	void Update () {
		if (i < 150)
			transform.RotateAround(transform.position, Vector3.up, 5);
		else
			transform.RotateAround(transform.position, Vector3.left, 5);
		i %= 300;
		i++;
	}
}
