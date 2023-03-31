using UnityEngine;

public class EventManager : MonoBehaviour
{
	private static UIManager ui;

	public float eventTime = 15;

	public bool startEventFlag = false;

	public float timer = 0;

	public int eventfrequency = 20;

	public float currentScore;

	GameManager manager;

	// Start is called before the first frame update
	void Start()
	{
		manager = GameManager.instance;
		InvokeRepeating("EventInitiation", eventfrequency, eventfrequency);
		ui = GameObject.Find("/UI").GetComponent<UIManager>();
	}

	// Update is called once per frame
	void Update()
	{
		timer += Time.deltaTime;
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
		int[] chances = { 50, 50 };
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
				Invoke("Event1End", eventTime);
				break;
			case 2:
				Event2Start();
				Invoke("Event2End", eventTime);
				break;
			default:
				break;
	}

}
	private void Event1Start()
	{
		ui.SetEventText("Get 50 points in 10 seconds.");
		ui.SetEventTextShown(true);

		currentScore = manager.currency;
	}
	private void Event1End()
	{
		ui.SetEventTextShown(false);
		if (currentScore + 50 <= manager.currency)
		{
			manager.currency += 50;
		}
		else
		{
			manager.currency -= 70;
		}
	}
	private void Event2Start()
	{
		ui.SetEventText("2x multiplier for generation.");
		ui.SetEventTextShown(true);
		manager.clickMultiplier = 2;
	}
	private void Event2End()
	{
		ui.SetEventTextShown(false);
		manager.clickMultiplier = 1;
	}
}
