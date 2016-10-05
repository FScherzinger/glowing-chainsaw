using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TangoUI : MonoBehaviour {

	public enum Tool{
		DRAGNDROP,
		DRAGROTATE,
		ANNOTATE,
		TEAMSPEAK
	};

	[SerializeField] private Texture dragNDropTool;
	[SerializeField] private Texture rotationTool;
	[SerializeField] private Texture warpTool;
	[SerializeField] private Texture annotationTool;
	[SerializeField] private Texture dragRotationtool;
	[SerializeField] private Texture teamSpeakTool;
	[SerializeField] private RawImage image;
	[SerializeField] private GameObject rotateLeft;
	[SerializeField] private GameObject rotateRight;

	public static Tool currentTool;

	// Use this for initialization
	void Awake () {
		this.gameObject.GetComponent<RawImage>().texture = dragNDropTool;
		//this.gameObject.GetComponentInChildren<Text>().text = "DRAG N DROP";
		currentTool = Tool.DRAGNDROP;
		rotateLeft.SetActive(false);
		rotateRight.SetActive(false);
	}

	public void Up(){
		switch(currentTool)
		{
		case Tool.ANNOTATE:
			image.texture = dragNDropTool;
			//if(TeamSpeakClient.started)
			//	this.gameObject.GetComponentInChildren<Text>().text = "DISCONNECT";
			//else
			//this.gameObject.GetComponentInChildren<Text>().text = "CONNECT";
			currentTool = Tool.DRAGNDROP;
			break;
		case Tool.DRAGNDROP:
			image.texture = dragRotationtool;
			//this.gameObject.GetComponentInChildren<Text>().text = "DRAGROTATE";
			currentTool = Tool.DRAGROTATE;
			rotateLeft.SetActive(true);
			rotateRight.SetActive(true);
			break;
		case Tool.DRAGROTATE:
			image.texture = annotationTool;
			//this.gameObject.GetComponentInChildren<Text>().text = "ANNOTATE";
			currentTool = Tool.ANNOTATE;
			rotateLeft.SetActive(false);
			rotateRight.SetActive(false);
			break;
		/*case Tool.TEAMSPEAK:
			image.texture = dragNDropTool;
			//this.gameObject.GetComponentInChildren<Text>().text = "DRAG N DROP";
			currentTool = Tool.DRAGNDROP;
			break;*/
		}
	}

	public void Down(){
		Debug.Log("down");
		switch(currentTool)
		{

		case Tool.DRAGNDROP:
			image.texture = annotationTool;
			//if(TeamSpeakClient.started)
			//	this.gameObject.GetComponentInChildren<Text>().text = "DISCONNECT";
			//else
			//this.gameObject.GetComponentInChildren<Text>().text = "CONNECT";
			currentTool = Tool.ANNOTATE;
			break;
		case Tool.ANNOTATE:
			image.texture = dragRotationtool;
			//this.gameObject.GetComponentInChildren<Text>().text = "DRAGROTATE";
			currentTool = Tool.DRAGROTATE;
			rotateLeft.SetActive(true);
			rotateRight.SetActive(true);
			break;
		case Tool.DRAGROTATE:
			image.texture = dragNDropTool;
			//this.gameObject.GetComponentInChildren<Text>().text = "DRAG N DROP";
			currentTool = Tool.DRAGNDROP;
			rotateLeft.SetActive(false);
			rotateRight.SetActive(false);
			break;
		/*case Tool.TEAMSPEAK:
			image.texture = annotationTool;
			//this.gameObject.GetComponentInChildren<Text>().text = "ANNOTATE";
			currentTool = Tool.ANNOTATE;
			break;*/
		}
	}
}
