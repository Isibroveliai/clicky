using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using EventHandler = System.Func<EventFrame, System.Collections.IEnumerator>;

public class EventManager : MonoBehaviour
{

	// Duration in seconds, how long each event lasts
	public float eventTime = 30;

	// Time in seconds, how frequently should event by tried to be picked
	public int eventFrequency = 20;

	public float eventChance = 0.5f;

	public GameObject eventTextbox;
	public TMP_Text eventText;
	private GameManager mng;

	private List<EventFrame> activeEvents;
	private List<Tuple<EventHandler, int>> eventWeights;

	private readonly System.Random rand = new();

	//no touchy
	private Animator anim;
	//private EventTrigger eventTrigger;
	private Button switchButton;
	private bool panelExtended = false;
	[SerializeField]
	private float delay = 1.5f;

	void Start()
	{
		activeEvents = new List<EventFrame>();
		eventWeights = new List<Tuple<EventHandler, int>>
		{
			Tuple.Create<EventHandler, int>(Event1, 1),
			Tuple.Create<EventHandler, int>(Event2, 1),
			Tuple.Create<EventHandler, int>(Event3, 1)
		};

		eventTextbox.SetActive(false);
		mng = GameManager.instance;
		InvokeRepeating(nameof(EventLauncher), eventFrequency, eventFrequency);

		anim = eventTextbox.GetComponentInChildren<Animator>();
		switchButton = eventTextbox.GetComponentInChildren<Button>();
		switchButton.onClick.AddListener(() => { OnButton(); });
	}

	private EventHandler PickRandomEvent()
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
		StartCoroutine(PlayDisableAnimAfterDelay(delay));

		return eventWeights.Last().Item1;
	}

	private void EventLauncher()
	{
		if (!(rand.NextDouble() < eventChance))
		{
			return;
		}
		var e = new EventFrame();
		var randEvent = PickRandomEvent();
		StartCoroutine(randEvent(e));
		activeEvents.Add(e);
	}

	private IEnumerator Event1(EventFrame e)
	{
		e.description = "Hard times... Resource generation is slower..";
		e.clickMultiplier = 0.5f;

		yield return new WaitForSeconds(eventTime);
	}

	private IEnumerator Event2(EventFrame e)
	{
		e.description = "Inspiration! You gain currency faster!";
		e.clickMultiplier = 5;

		yield return new WaitForSeconds(eventTime);
	}

	private IEnumerator Event3(EventFrame e)
	{
		e.description = "Eureka! Your research accelerates!";
		e.researchMultiplier = 2;

		yield return new WaitForSeconds(eventTime);
	}

	public void ShowEventText(string text)
	{
		eventText.text = text;
		eventTextbox.SetActive(true);
	}

	public void HideEventText()
	{
		eventTextbox.SetActive(false);
	}

	//-----------leave this alone pls :)---------------------
	public void EnableAnim()
	{
		ResetTriggers("Enable", "Disable");
		anim.SetTrigger("Enable");
	}
	public void DisableAnim()
	{
		ResetTriggers("Enable", "Disable");
		anim.SetTrigger("Disable");
	}
	public void OnButton()
	{

		if (panelExtended)
		{
			DisableAnim();
		}
		else
		{
			EnableAnim();
		}
		panelExtended = !panelExtended;
	}

	public void ResetTriggers(params string[] triggers)
	{
		foreach(string str in triggers)
		{
			anim.ResetTrigger(str);
		}
	}

	IEnumerator PlayDisableAnimAfterDelay(float delay)
	{
		print("before");
		yield return new WaitForSeconds(delay);
		print("after");
		DisableAnim();
	}

	//-------------------------------------------------------



}
