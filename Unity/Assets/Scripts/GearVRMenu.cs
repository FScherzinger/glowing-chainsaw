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
		ANNOTATE
	};

	[SerializeField] private VRInput vRInput;  
	[SerializeField] private Texture dragNDropTool;
	[SerializeField] private Texture rotationTool;
	[SerializeField] private Texture warpTool;
	[SerializeField] private Texture annotationTool;
	[SerializeField] private RawImage image;
	private int i = 0;

	public static Tool currentTool;

	public void Awake(){
		image.texture = dragNDropTool;
		this.gameObject.GetComponentInChildren<Text>().text = "DRAG N DROP";
		currentTool = Tool.DRAGNDROP;
	}

	private void OnEnable()
	{
		vRInput.OnSwipe += HandleSwipe;
		//vRInput.OnDoubleClick += HandleDoubleClick;
	}


	private void OnDisable()
	{
		vRInput.OnSwipe -= HandleSwipe;
		//vRInput.OnDoubleClick -= HandleDoubleClick;
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
			//DRAGNDROP -> ROTATE -> WARP -> ANNOTATE
			switch(currentTool)
			{
				case Tool.ANNOTATE:
					image.texture = dragNDropTool;
					this.gameObject.GetComponentInChildren<Text>().text = "DRAG N DROP";
					currentTool = Tool.DRAGNDROP;
					break;
				case Tool.DRAGNDROP:
					image.texture = rotationTool;
					this.gameObject.GetComponentInChildren<Text>().text = "ROTATE";
					currentTool = Tool.ROTATE;
					break;
				case Tool.ROTATE:
					image.texture = warpTool;					
					this.gameObject.GetComponentInChildren<Text>().text = "WARP";
					currentTool = Tool.WARP;
					break;
				case Tool.WARP:
					image.texture = annotationTool;
					this.gameObject.GetComponentInChildren<Text>().text = "ANNOTATE";
					currentTool = Tool.ANNOTATE;
					break;
			}
			break;
		case VRInput.SwipeDirection.DOWN:
			//DRAGNDROP <- ROTATE <- WARP <- ANNOTATE
			switch(currentTool)
			{
				case Tool.WARP:
					image.texture = rotationTool;
					this.gameObject.GetComponentInChildren<Text>().text = "ROTATE";
					currentTool = Tool.ROTATE;
					break;
				case Tool.DRAGNDROP:
					image.texture = annotationTool;
					this.gameObject.GetComponentInChildren<Text>().text = "ANNOTATE";
					currentTool = Tool.ANNOTATE;
					break;
				case Tool.ROTATE:
					image.texture = dragNDropTool;
					this.gameObject.GetComponentInChildren<Text>().text = "DRAG N DROP";
					currentTool = Tool.DRAGNDROP;
					break;
				case Tool.ANNOTATE:
					image.texture = warpTool;
					this.gameObject.GetComponentInChildren<Text>().text = "WARP";
					currentTool = Tool.WARP;
					break;
			}
			break;
		}
	}

	private void HandleDoubleClick(){
		switch(currentTool)
		{
		case Tool.ANNOTATE:
			image.texture = dragNDropTool;
			this.gameObject.GetComponentInChildren<Text>().text = "DRAG N DROP";
			currentTool = Tool.DRAGNDROP;
			break;
		case Tool.DRAGNDROP:
			image.texture = rotationTool;
			this.gameObject.GetComponentInChildren<Text>().text = "ROTATE";
			currentTool = Tool.ROTATE;
			break;
		case Tool.ROTATE:
			image.texture = warpTool;					
			this.gameObject.GetComponentInChildren<Text>().text = "WARP";
			currentTool = Tool.WARP;
			break;
		case Tool.WARP:
			image.texture = annotationTool;
			this.gameObject.GetComponentInChildren<Text>().text = "ANNOTATE";
			currentTool = Tool.ANNOTATE;
			break;
		}
	}
}