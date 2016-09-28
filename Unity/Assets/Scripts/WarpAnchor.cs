using UnityEngine;
using System.Collections;
using VRStandardAssets.Utils;
using UnityEngine.UI;

public class WarpAnchor : MonoBehaviour {

	[SerializeField] private VRInput vRInput;
	[SerializeField] private Image reticle;
	const float max_z = 0.4f;
	const float min_z = -2.4f;
	const float max_x = 2.6f;
	const float min_x = -0.2f;


	private void OnEnable()
	{
		vRInput.OnDoubleClick += HandleClick;
	}


	private void OnDisable()
	{
		vRInput.OnDoubleClick -= HandleClick;
	}

	private void HandleClick(){
		if(GearVRMenu.currentTool == GearVRMenu.Tool.WARP){
			Vector3 new_pos = reticle.transform.position;
			Vector3 curr_pos = this.gameObject.transform.position;
			if(new_pos.x < min_x)
				new_pos.x = min_x;
			if(new_pos.x > max_x)
				new_pos.x = max_x;
			if(new_pos.z < min_z)
				new_pos.z = min_z;
			if(new_pos.z > max_z)
				new_pos.z = max_z;
			this.gameObject.transform.position = new Vector3(new_pos.x, curr_pos.y, new_pos.z);
		} else if(GearVRMenu.currentTool == GearVRMenu.Tool.TEAMSPEAK){
			if(TeamSpeakClient.started)
				this.gameObject.GetComponent<TeamSpeakVoiceChat>().Disconnect();
			else
				this.gameObject.GetComponent<TeamSpeakVoiceChat>().Connect();
		}
	}
		
}
