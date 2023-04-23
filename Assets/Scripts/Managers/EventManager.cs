using TMPro;
using UnityEngine;

public class EventManager : MonoBehaviour
{
	// Duration in seconds, how long each event lasts
	public float eventTime = 30;

	// Time in seconds, how frequently should event by tried to be picked
	public int eventFrequency = 20;

	// Indicates if an event is currently active
	private bool startEventFlag = false;

	public GameObject eventTextbox;
	public TMP_Text eventText;
	private GameManager mng;

	void Start()
	{
		eventTextbox.SetActive(false);
		mng = GameManager.instance;
		InvokeRepeating(nameof(EventInitiation), eventFrequency, eventFrequency);
	}

	private void EventInitiation()
	{
		var random = new System.Random();
		if (random.Next(10) > 5)
		{
			startEventFlag = true;
		}
		if (startEventFlag)
		{
			startEventFlag = false;
			EventPicker();
		}
	}

	private void EventPicker()
	{
		var random = new System.Random();
		int[] chances = { 33, 33, 33 };
		int totalRatio = 0;

		foreach (int c in chances)
			totalRatio += c;
		int x = random.Next(0, totalRatio);

		int iteration = 0;
		foreach (int c in chances)
		{
			iteration++;
			if ((x -= c) < 0)
				break;
		}

		switch (iteration)
		{
			case 1:
				Event1Start();
				Invoke(nameof(Event1End), eventTime);
				break;
			case 2:
				Event2Start();
				Invoke(nameof(Event2End), eventTime);
				break;
			case 3:
				Event3Start();
				Invoke(nameof(Event3End), eventTime);
				break;
			default:
				break;
		}

	}
	private void Event1Start()
	{
		ShowEventText("Hard times... Resource generation is slower..");

		mng.currencyPerClick = new HugeNumber(0.5f);
	}
	private void Event1End()
	{
		HideEventText();
		mng.currencyPerClick = new HugeNumber(1);
	}
	private void Event2Start()
	{
		ShowEventText("Inspiration! You gain currency faster!");
		mng.currencyPerClick = new HugeNumber(5);
	}
	private void Event2End()
	{
		HideEventText();
		mng.currencyPerClick = new HugeNumber(1);
	}
	private void Event3Start()
	{
		ShowEventText("Eureka! Your research accelerates!");
		mng.researchProduction *= 2;
	}
	private void Event3End()
	{
		HideEventText();
		mng.researchProduction /= 2;
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
}
