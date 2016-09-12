using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AttachmentScript : MonoBehaviour {
	[SerializeField] private Image reticle;
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = reticle.transform.position;
		pos.y += this.gameObject.transform.localScale.y;
		this.gameObject.transform.position = pos;
	}
}
