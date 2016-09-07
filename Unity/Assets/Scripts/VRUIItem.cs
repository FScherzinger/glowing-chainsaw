using System;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;

public class VRUIItem: MonoBehaviour
{
	[SerializeField] private VRInput m_VRInput;  
	[SerializeField] private Texture m_FirstTool;
	[SerializeField] private Texture m_SecondTool;
	[SerializeField] private Texture m_ThirdTool;
	[SerializeField] private RawImage image;
	private int i = 0;


	private void OnEnable()
	{
		m_VRInput.OnSwipe += HandleSwipe;
	}


	private void OnDisable()
	{
		m_VRInput.OnSwipe -= HandleSwipe;
	}

	private void HandleSwipe(VRInput.SwipeDirection swipeDirection)
	{
		switch (swipeDirection)
		{
		case VRInput.SwipeDirection.NONE:
			break;
		case VRInput.SwipeDirection.UP:
			break;
		case VRInput.SwipeDirection.DOWN:
			break;
		case VRInput.SwipeDirection.LEFT:
			i--;
			break;
		case VRInput.SwipeDirection.RIGHT:
			i++;
			break;
		}

		i %= 3;
		switch(i){
		case 0:
			image.texture = m_FirstTool;
			break;
		case 1:
			image.texture = m_SecondTool;
			break;
		case 2:
			image.texture = m_ThirdTool;
			break;
		}
	}
}