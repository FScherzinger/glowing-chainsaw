using System;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;

public class GearVRMenu: MonoBehaviour
{
	public enum Tool{
		WARP,
		DRAGNDROP,
		ROTATE,
		DRAGROTATE,
		ANNOTATE,
		TEAMSPEAK
	};

	[SerializeField] private VRInput vRInput;  
	[SerializeField] private Texture dragNDropTool;
	[SerializeField] private Texture rotationTool;
	[SerializeField] private Texture warpTool;
	[SerializeField] private Texture annotationTool;
	[SerializeField] private Texture dragRotationtool;
	[SerializeField] private Texture teamSpeakTool;
	[SerializeField] private RawImage image;

	public static Tool currentTool;

	public void Awake(){
		image.texture = dragNDropTool;
		this.gameObject.GetComponentInChildren<Text>().text = "DRAG N DROP";
		currentTool = Tool.DRAGNDROP;
	}

	private void OnEnable()
	{
		vRInput.OnSwipe += HandleSwipe;
	}


	private void OnDisable()
	{
		vRInput.OnSwipe -= HandleSwipe;
	}

	private void HandleSwipe(VRInput.SwipeDirection swipeDirection)
	{
		switch (swipeDirection)
		{
		case VRInput.SwipeDirection.NONE:
		case VRInput.SwipeDirection.LEFT:
		case VRInput.SwipeDirection.RIGHT:
			break;
		case VRInput.SwipeDirection.UP:
			//DRAGNDROP -> WARP -> DRAGROTATE -> ANNOTATE -> TEAMSPEAK
			switch(currentTool)
			{
				case Tool.ANNOTATE:
					image.texture = teamSpeakTool;
					//if(TeamSpeakClient.started)
					//	this.gameObject.GetComponentInChildren<Text>().text = "DISCONNECT";
					//else
						this.gameObject.GetComponentInChildren<Text>().text = "CONNECT";
					currentTool = Tool.TEAMSPEAK;
					break;
				case Tool.DRAGNDROP:
					image.texture = warpTool;					
					this.gameObject.GetComponentInChildren<Text>().text = "WARP";
					currentTool = Tool.WARP;
					break;
				case Tool.WARP:
				image.texture = dragRotationtool;
					this.gameObject.GetComponentInChildren<Text>().text = "DRAGROTATE";
					currentTool = Tool.DRAGROTATE;
					break;
				case Tool.DRAGROTATE:
					image.texture = annotationTool;
					this.gameObject.GetComponentInChildren<Text>().text = "ANNOTATE";
					currentTool = Tool.ANNOTATE;
					break;
				case Tool.TEAMSPEAK:
					image.texture = dragNDropTool;
					this.gameObject.GetComponentInChildren<Text>().text = "DRAG N DROP";
					currentTool = Tool.DRAGNDROP;
					break;
			}
			break;
		case VRInput.SwipeDirection.DOWN:
			//DRAGNDROP <- WARP <- DRAGROTATE <- ANNOTATE <- TEAMSPEAK
			switch(currentTool)
			{
				
				case Tool.DRAGNDROP:
					image.texture = teamSpeakTool;
					//if(TeamSpeakClient.started)
					//	this.gameObject.GetComponentInChildren<Text>().text = "DISCONNECT";
					//else
						this.gameObject.GetComponentInChildren<Text>().text = "CONNECT";
					currentTool = Tool.TEAMSPEAK;
					break;
				case Tool.WARP:
					image.texture = dragNDropTool;
					this.gameObject.GetComponentInChildren<Text>().text = "DRAG N DROP";
					currentTool = Tool.DRAGNDROP;
					break;
				case Tool.ANNOTATE:
					image.texture = dragRotationtool;
					this.gameObject.GetComponentInChildren<Text>().text = "DRAGROTATE";
					currentTool = Tool.DRAGROTATE;
					break;
				case Tool.DRAGROTATE:
					image.texture = warpTool;
					this.gameObject.GetComponentInChildren<Text>().text = "WARP";
					currentTool = Tool.WARP;
					break;
				case Tool.TEAMSPEAK:
					image.texture = annotationTool;
					this.gameObject.GetComponentInChildren<Text>().text = "ANNOTATE";
					currentTool = Tool.ANNOTATE;
					break;
			}
			break;
		}
	}
}