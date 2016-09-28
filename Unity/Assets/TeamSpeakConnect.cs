using UnityEngine;
using System.Collections;

public class TeamSpeakConnect : MonoBehaviour {

	[SerializeField] private PCInput PcInput;

	public void OnEnable(){
		PcInput.OnKeyFour += OnConnect;
	}

	public void OnDisable(){
		PcInput.OnKeyFour -= OnConnect;
	}

	void OnConnect(){
		if(TeamSpeakClient.started)
			gameObject.GetComponent<TeamSpeakVoiceChat>().Disconnect();
		else
			gameObject.GetComponent<TeamSpeakVoiceChat>().Connect();
	}
}
