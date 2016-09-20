using UnityEngine;
using System.Collections;

public class GetHeadPos : MonoBehaviour {

    public GameObject head;
	// Use this for initializatio
	
	// Update is called once per frame
	void Update () {
        if (head.activeInHierarchy)
        {
            transform.position = head.transform.position;
            transform.rotation = head.transform.rotation;
        }
	}
}
