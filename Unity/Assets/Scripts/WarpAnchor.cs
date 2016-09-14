using UnityEngine;
using System.Collections;
using VRStandardAssets.Utils;
using UnityEngine.UI;

public class WarpAnchor : MonoBehaviour {

	[SerializeField] private VRInput vRInput;
	[SerializeField] private Image reticle;

	private void OnEnable()
	{
		vRInput.OnClick += HandleClick;
	}


	private void OnDisable()
	{
		vRInput.OnClick -= HandleClick;
	}

	private void HandleClick(){
		if(GearVRMenu.currentTool == GearVRMenu.Tool.WARP){
			Vector3 new_pos = reticle.transform.position;
			Vector3 curr_pos = this.gameObject.transform.position;
			this.gameObject.transform.position = new Vector3(new_pos.x, curr_pos.y, new_pos.z);
		}
	}
		
}
