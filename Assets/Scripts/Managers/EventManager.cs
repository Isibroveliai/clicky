using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

using MyEventHandler = System.Func<EventFrame, System.Collections.IEnumerator>;

public class EventManager : MonoBehaviour // PO SCENE RELOAD BUNA NEGERAI SU SINGLETON KLASE PLS NEKEISKIT
{
	public static EventManager instance;
	// Time in seconds, how frequently should event by tried to be picked
	public float eventFrequency = 20.0f;

	public float eventChance = 0.5f;

	public List<Upgrade> upgradeUnlockFromEvent;
	public List<ResearchNode> researchUnlockFromEvent;
	public GameObject eventNotificationContainer;
	public GameObject eventTextbox;
	public TMP_Text eventText;
	private GameManager mng;

	public List<Tuple<GameObject, EventFrame, MyEventHandler>> activeEvents;
	private List<Tuple<MyEventHandler, int>> eventWeights;
	private IEnumerator currentEventNotification;

	private readonly System.Random rand = new();

	private Animator anim;

	public static event Action<EventFrame> OnEventStarted;
	public static event Action<EventFrame> OnEventFinished;

	[HideInInspector]
	public float researchMultiplier = 1;
	[HideInInspector]
	public float clickMultiplier = 1;
	[HideInInspector]
	public float maxEnergy = 1;
	
	[HideInInspector]
	public float generationMultiplier = 1;

	public void Awake()
	{
		if (instance != null && instance != this )
		{
			Destroy(this);
			return;
		}
		instance = this ;

		Setup();
	
	}
	public  void Setup()
	{
		activeEvents = new List<Tuple<GameObject, EventFrame, MyEventHandler>>();
		eventWeights = new List<Tuple<MyEventHandler, int>>
		{
			// Tuple.Create<MyEventHandler, int>(EventLowerCurrencyGen, 10),
			// Tuple.Create<MyEventHandler, int>(EventLowerClickCurrencyGen, 12),
			// Tuple.Create<MyEventHandler, int>(EventHigherClickCurrencyGen, 12),
			// Tuple.Create<MyEventHandler, int>(EventHigherCurrencyGen, 10),
			// Tuple.Create<MyEventHandler, int>(EventEverythingBetter, 8),
			// Tuple.Create<MyEventHandler, int>(EventEnergyOverload, 5),
			// Tuple.Create<MyEventHandler, int>(EventGiveMoney, 1),
			//Tuple.Create<MyEventHandler, int>(EventUnlockUpgrade, 2),
			//Tuple.Create<MyEventHandler, int>(EventSystemFailure, 1),
			Tuple.Create<MyEventHandler, int>(EventDisableRandomUpgrades, 5),
			
			
			//Tuple.Create<MyEventHandler, int>(Event5, 1)
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
		
		var notActive = eventWeights.Where(e => activeEvents.All(e1 => { return !e1.Item3.Equals(e.Item1); })).ToList();

		foreach (var entry in notActive)
		{
			print(string.Format("{0}", entry.Item2));
			weightsSum += entry.Item2;
		}

		var randValue = rand.Next(weightsSum);
		foreach (var entry in notActive)
		{
			if (randValue <= entry.Item2)
			{
				return entry.Item1;
			}
			randValue -= entry.Item2;
		}

		return notActive.Last().Item1;
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
		var notifObject = GetUnusedNotificationObject();
		var entry = Tuple.Create(notifObject, e, handler);
		activeEvents.Add(entry);

		var enumerator = handler(e);
		OnEventStarted?.Invoke(e);
		enumerator.MoveNext();

		notifObject.SetActive(true);
		StopCurrentNotificationIfExists();
		var notificationCoroutine = TemporarilyShowDescription(e.description, 1.5f);
		StartCoroutine(notificationCoroutine);
		currentEventNotification = notificationCoroutine;

		yield return enumerator;

		OnEventFinished?.Invoke(e);
		activeEvents.Remove(entry);
		notifObject.SetActive(false);

		ApplyChanges();
	}

	private int ApplyChanges()
	{
		researchMultiplier = 1;
		clickMultiplier = 1;
		generationMultiplier = 1;
		maxEnergy = 0;
		mng.maxEnergy = mng.baseMaxEnergy;
		foreach (var entry in activeEvents)
		{
			var frame = entry.Item2;
			clickMultiplier *= frame.clickMultiplier;
			researchMultiplier *= frame.researchMultiplier;
			maxEnergy += frame.energyCap;
			generationMultiplier *= frame.generationMultiplier;
			foreach (var upgrade in frame.upgrades)
			{
				GameManager.instance.UnlockUpgrade(upgrade);
			}
			foreach (var research in frame.research)
			{
				GameManager.instance.UnlockResearchForFreeNow(research);
			}
		}
		mng.maxEnergy -= maxEnergy;
		return 0;
	}

	private Upgrade GetUpgrade(string path)
	{
		var upgrade = Resources.Load<Upgrade>($"Upgrades/{path}");
		if (upgrade == null)
		{
			Debug.LogWarning($"Failed to find upgrade '{path}'");
		}
		return upgrade;
	}

	private ResearchNode GetResearch(string path)
	{
		var research = Resources.Load<ResearchNode>($"Research/{path}");
		if (research == null)
		{
			Debug.LogWarning($"Failed to find research node '{path}'");
		}
		return research;
	}

	private IEnumerator EventLowerClickCurrencyGen(EventFrame e)
	{
		e.description = "Exhaustion... Currency generation from click is slower.";
		e.clickMultiplier = 0.5f;

		yield return ApplyChanges();
		yield return new WaitForSeconds(10);
	}
		private IEnumerator EventLowerCurrencyGen(EventFrame e)
	{
		e.description = "Slow day... Currency generation from upgrades is slower";
		e.generationMultiplier = 0.5f;

		yield return ApplyChanges();
		yield return new WaitForSeconds(10);
	}

	private IEnumerator EventHigherCurrencyGen(EventFrame e)
	{
		e.description = "Overtime! Increased currency generation from upgrades.";
		e.generationMultiplier = 1.5f;

		yield return ApplyChanges();
		yield return new WaitForSeconds(15);
	}
	private IEnumerator EventHigherClickCurrencyGen(EventFrame e)
	{
		e.description = "Motivation! Your click currency gain is faster!";
		e.clickMultiplier = 1.5f;

		yield return ApplyChanges();
		yield return new WaitForSeconds(15);
	}

	private IEnumerator EventIncreaseResearch(EventFrame e)
	{
		e.description = "Eureka! Your research accelerates.";
		e.researchMultiplier = 2;

		yield return ApplyChanges();
		yield return new WaitForSeconds(30);
	}

	private IEnumerator EventUnlockUpgrade(EventFrame e)
	{
		e.description = "One mans trash is anothers treasure. Coal upgrade unlocked.";

		e.upgrades.Add(GetUpgrade("Coal"));

		yield return ApplyChanges();
		yield return new WaitForSeconds(1);
	}
	private IEnumerator EventEnergyOverload(EventFrame e)
	{
		e.description = "Energy crisis! The energy cap is halved.";
		//e.upgrades.Add(GetUpgrade("Coal"));
		e.energyCap = mng.maxEnergy / 2;

		yield return ApplyChanges();
		yield return new WaitForSeconds(5);
	}
	private IEnumerator EventUnlockResearch(EventFrame e)
	{
		e.description = "";
		e.research.Add(GetResearch("Main/2_2_research"));

		yield return ApplyChanges();
		yield return new WaitForSeconds(1);
	}
	private IEnumerator EventDisableRandomUpgrades(EventFrame e)
	{
		
		e.description = "Reboot... Some purchased upgrades need to be reenabled.";
		if(mng.upgradesInShop.Count() == 0) yield break; //still plays notif but i cannot be bothered to fix so just leave this coroutine
		System.Random r = new System.Random();

		if(mng.upgradesInShop.Count() > 1)
		{
			int count = r.Next(1, mng.upgradesInShop.Count() - 1);

			for(int i = 0; i < count; i ++)
			{
				int ind = r.Next(0,  mng.upgradesInShop.Count() );
				if(mng.data.disabledUpgrades.Contains(mng.upgradesInShop[ind].id))
					continue;
				mng.DisableUpgrade(mng.upgradesInShop[ind]);
				var tab = Resources.FindObjectsOfTypeAll<UpgradeTab>();
				print(tab[0]);
				tab[0].DisableButton(mng.upgradesInShop[ind]);
			}
		}
		else
		{
			mng.DisableUpgrade(mng.upgradesInShop[0]);
			var tab = Resources.FindObjectsOfTypeAll<UpgradeTab>();
				print(tab[0]);
				tab[0].DisableButton(mng.upgradesInShop[0]);
		} 
		
		yield return ApplyChanges();
		yield return new WaitForSeconds(1);
	}

	private IEnumerator EventGiveMoney(EventFrame e)
	{
		e.description = "Government financing! Currency has been added to your account.";
		mng.data.currency *= 1.5f;
		yield return new WaitForSeconds(1);
	}

	private IEnumerator EventSystemFailure(EventFrame e)
	{
		e.description = "System failure... Currency generation currently unavailable.";
		e.generationMultiplier = 0;
		e.clickMultiplier = 0;
		yield return ApplyChanges();
		yield return new WaitForSeconds(5);
	}

	private IEnumerator EventEverythingBetter(EventFrame e)
	{
		e.description = "A bit of everything! Every stat increased.";
		e.generationMultiplier = 1.15f;
		e.clickMultiplier = 1.15f;
		e.researchMultiplier = 1.15f;
		yield return ApplyChanges();
		yield return new WaitForSeconds(30);
	}
	
}
