using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Interface : MonoBehaviour
{
	[SerializeField]
	private CanvasScaler scaler;

	[SerializeField]
	private int referenceHeight = 600;

	private int lastScreenHeight = 0;

	private Coroutine autoSelecter;

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

	public void SetAutoSelect(Button bs)
	{
		bs.Select();
		autoSelecter = StartCoroutine(PersistentSelect(bs));
	}

	public void StopAutoSelect()
	{
		if (autoSelecter != null)
		{
			StopCoroutine(autoSelecter);
		}
	}

	private IEnumerator PersistentSelect(Button bs)
	{
		StopAutoSelect();
		yield return null;

		while (bs.gameObject.activeInHierarchy)
		{
			if (EventSystem.current.currentSelectedGameObject != bs.gameObject)
			{
				bs.Select();
			}
			yield return null;
		}
	}
}
