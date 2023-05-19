using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

using MyEventHandler = System.Func<EventFrame, System.Collections.IEnumerator>;

public class EventManager : Singleton<EventManager>
{
	// Time in seconds, how frequently should event by tried to be picked
	public float eventFrequency = 20.0f;

	public float eventChance = 0.5f;

	public GameObject eventNotificationContainer;
	public GameObject eventTextbox;
	public TMP_Text eventText;
	private GameManager mng;

	public List<Tuple<GameObject, EventFrame>> activeEvents;
	private List<Tuple<MyEventHandler, int>> eventWeights;
	private IEnumerator currentEventNotification;

	private readonly System.Random rand = new();

	private Animator anim;

	public static event Action<EventFrame> OnEventStarted;
	public static event Action<EventFrame> OnEventFinished;

	private float researchMultiplier = 1;
	private float clickMultiplier = 1;
	
	public override void Setup()
	{
		activeEvents = new List<Tuple<GameObject, EventFrame>>();
		eventWeights = new List<Tuple<MyEventHandler, int>>
		{
			Tuple.Create<MyEventHandler, int>(Event1, 1),
			Tuple.Create<MyEventHandler, int>(Event2, 1),
			Tuple.Create<MyEventHandler, int>(Event3, 1)
		};

		mng = GameManager.instance;
		InvokeRepeating(nameof(EventLauncher), eventFrequency, eventFrequency);

		anim = eventTextbox.GetComponentInChildren<Animator>();

		for (int i = 0; i < eventNotificationContainer.transform.childCount; i++)
		{
			var child = eventNotificationContainer.transform.GetChild(i);
			child.gameObject.SetActive(false);
		}
	}

	public void StopCurrentNotificationIfExists()
	{
		if (currentEventNotification != null)
		{
			StopCoroutine(currentEventNotification);
			currentEventNotification = null;
		}
	}

	public void OnNotificationPointerEnter(GameObject obj)
	{
		EventFrame eventFrame = null;
		foreach (var entry in activeEvents)
		{
			if (entry.Item1 == obj)
			{
				eventFrame = entry.Item2;
				break;
			}
		}
		if (eventFrame == null) return;

		StopCurrentNotificationIfExists();
		ShowDescription(eventFrame.description);
		
	}

	public void OnNotificationPointerExit(GameObject obj)
	{
		HideDescription();
	}

	private MyEventHandler PickRandomEvent()
	{
		int weightsSum = 0;
		foreach (var entry in eventWeights)
		{
			weightsSum += entry.Item2;
		}

		var randValue = rand.Next(weightsSum);
		foreach (var entry in eventWeights)
		{
			if (randValue <= entry.Item2)
			{
				return entry.Item1;
			}
			randValue -= entry.Item2;
		}

		return eventWeights.Last().Item1;
	}

	private void EventLauncher()
	{
		if (!(rand.NextDouble() < eventChance))
		{
			return;
		}

		if (activeEvents.Count >= eventNotificationContainer.transform.childCount)
		{
			Debug.Log("Attempt create event, while at limit");
			return;
		}
		
		var randEvent = PickRandomEvent();
		if (randEvent == null) return;
		StartCoroutine(EventWrapper(randEvent));
	}

	private void ShowDescription(string text)
	{
		eventText.text = text;
		anim.ResetTrigger("Show");
		anim.SetTrigger("Show");
	}

	public void HideDescription()
	{
		anim.ResetTrigger("Hide");
		anim.SetTrigger("Hide");
	}

	private IEnumerator TemporarilyShowDescription(string text, float duration)
	{
		ShowDescription(text);
		yield return new WaitForSeconds(duration);
		HideDescription();
	}

	private GameObject GetUnusedNotificationObject()
	{
		for (int i = 0; i < eventNotificationContainer.transform.childCount; i++)
		{
			var child = eventNotificationContainer.transform.GetChild(i);
			if (!child.gameObject.activeSelf)
			{
				return child.gameObject;
			}
		}
		return null;
	}

	private IEnumerator EventWrapper(MyEventHandler handler)
	{
		var e = new EventFrame();
		var coroutine = handler(e);
		coroutine.MoveNext();

		int i = activeEvents.Count-1;
		var notifObject = GetUnusedNotificationObject();
		notifObject.SetActive(true);
		var entry = Tuple.Create(notifObject, e);
		activeEvents.Add(entry);
		OnEventStarted?.Invoke(e);

		StopCurrentNotificationIfExists();
		var notificationCoroutine = TemporarilyShowDescription(e.description, 1.5f);
		StartCoroutine(notificationCoroutine);
		currentEventNotification = notificationCoroutine;

		yield return coroutine;

		OnEventFinished?.Invoke(e);
		activeEvents.Remove(entry);
		notifObject.SetActive(false);

		ApplyChanges(e);
	}

	private int ApplyChanges(EventFrame e)
	{
		researchMultiplier = 1;
		clickMultiplier = 1;
		foreach (var entry in activeEvents)
		{
			var frame = entry.Item2;
			clickMultiplier *= frame.clickMultiplier;
			researchMultiplier *= frame.researchMultiplier;
		}

		return 0;
	}

	private IEnumerator Event1(EventFrame e)
	{
		e.description = "Hard times... Resource generation is slower..";
		e.clickMultiplier = 0.5f;

		yield return ApplyChanges(e);
		yield return new WaitForSeconds(30);
	}

	private IEnumerator Event2(EventFrame e)
	{
		e.description = "Inspiration! You gain currency faster!";
		e.clickMultiplier = 5;

		yield return ApplyChanges(e);
		yield return new WaitForSeconds(30);
	}

	private IEnumerator Event3(EventFrame e)
	{
		e.description = "Eureka! Your research accelerates!";
		e.researchMultiplier = 2;

		yield return ApplyChanges(e);
		yield return new WaitForSeconds(30);
	}
}
