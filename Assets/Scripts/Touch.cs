using UnityEngine;
using UnityEngine.EventSystems;

public class Touch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
	public bool touchState = false;
	public void OnPointerDown(PointerEventData eventData)
	{
		touchState = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		touchState = false;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (Input.touches.Length > 0)
		{
			touchState = true;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		touchState = false;
	}
}
