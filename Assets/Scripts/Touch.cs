using UnityEngine;
using UnityEngine.EventSystems;

public class Touch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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
}
