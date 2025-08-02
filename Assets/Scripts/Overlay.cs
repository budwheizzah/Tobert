using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Overlay : MonoBehaviour
{
	[SerializeField]
	private CanvasScaler scaler;

	[SerializeField]
	private int referenceHeight = 600;

	private int lastScreenHeight = 0;

	void Start()
	{
		ScaleCanvas();
	}

	private void Update()
	{
		if (lastScreenHeight != Screen.height)
		{
			ScaleCanvas();
		}
	}

	private void ScaleCanvas()
	{
		float coef = (float)Screen.height / (float)referenceHeight;
		scaler.scaleFactor = coef;
		lastScreenHeight = Screen.height;
	}
}
