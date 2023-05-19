using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventNotification : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public void OnPointerEnter(PointerEventData eventData)
	{
		EventManager.instance.OnNotificationPointerEnter(gameObject);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		EventManager.instance.OnNotificationPointerExit(gameObject);
	}
}
