using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	private static UIManager ui;

	// TODO: Make this read-only through editor
	[SerializeField]
	public float score = 0;

	public Dictionary<string, int> upgradeCounts;
	public List<string> researchUnlocks;

	public float currentGeneration = 0;

	public float scoreReductionRate = 1f; // rate of which score reduces

	// TODO: Make this read-only through editor
	public float currentEnergy = 100; // if reaches 0, game lost

	public float maxEnergy = 100; // upgradable

	public float energyRegenerationRate = 0.001f;

	public bool scoreThresholdReached = false; // 100 for prototype?
	public float clickMultiplier = 1;

	public GameManager()
	{
		upgradeCounts = new Dictionary<string, int>();
	}

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(this);
			return;
		}
		instance = this;
		Setup();
	}

	void Setup()
	{
		ui = GameObject.Find("/UI").GetComponent<UIManager>();
		ui.UpdateUpgradeDescription("");
	}

	void Update()
	{
		if (score > 100) // number changeable
			scoreThresholdReached = true;

		// Score += CurrentGeneration * Time.deltaTime;
		if (scoreThresholdReached)
		{
			currentEnergy -= scoreReductionRate * Time.deltaTime;
		}

		if (currentEnergy < 0)
		{
			ui.SetGameOverShown(true);
		}
		ui.UpdateScoreDisplay((ulong)score);
		ui.UpdateEnergyDisplay(currentEnergy / maxEnergy);
	}

	public void GenerateCurrency()
	{
		score += clickMultiplier;
		currentEnergy = Mathf.Min(maxEnergy, currentEnergy + energyRegenerationRate);
	}

	public void RestartScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
