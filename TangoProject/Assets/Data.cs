using UnityEngine;
using System.Collections;

public class Data : MonoBehaviour {

	public void Awake(){
		DontDestroyOnLoad(transform.gameObject);
	}
}
