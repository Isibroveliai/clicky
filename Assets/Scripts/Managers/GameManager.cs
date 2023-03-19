using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	private static UIManager ui;

	public List<Upgrade> allUpgrades;

	// TODO: Make this read-only through editor
	[SerializeField]
	public float score = 0;

	public Dictionary<string, int> upgradeCounts;
	public float currentGeneration = 0;
	public float scoreReductionRate = 1f; // rate of which score reduces
										  // TODO: Make this read-only through editor
	[ReadOnly]
	public float currentEnergy = 0; // current consumption of devices
	[ReadOnly]
	public float maxEnergy = 1000; // upgradable
	//[ReadOnly]
	//public int energyRegenerationRate = 0.001f;

	public bool scoreThresholdReached = false; // 100 for prototype?
	public float clickMultiplier = 1;

	public List<Upgrade> unlockedUpgrades;
	public List<ResearchNode> unlockedResearch;

	public static event Action<Upgrade> OnUpgradeBought;

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
		ui.UpdateEnergyDisplay(currentEnergy, maxEnergy);
		//foreach (var upgrade in Resources.LoadAll<Upgrade>("Upgrades"))
		//{
		//	UnlockUpgrade(upgrade);
		//}
	}

	void Update()
	{
		if (score > 100) // number changeable
			scoreThresholdReached = true;

		score += currentGeneration * Time.deltaTime;
		//if (scoreThresholdReached)
		//{
		//	currentEnergy -= scoreReductionRate * Time.deltaTime;
		//}

		//if (currentEnergy < 0)
		//{
		//	ui.SetGameOverShown(true);
		//}
		ui.UpdateScoreDisplay((ulong)score);
		//ui.UpdateEnergyDisplay(currentEnergy / maxEnergy);
	}

	public void GenerateCurrency()
	{
		score += clickMultiplier;
		//currentEnergy = Mathf.Min(maxEnergy, currentEnergy + energyRegenerationRate);
	}

	public void RestartScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void UnlockUpgrade(Upgrade upgrade)
	{
		if (unlockedUpgrades.Contains(upgrade)) return;

		unlockedUpgrades.Add(upgrade);
		ui.AppendUpgradeButton(upgrade);
	}

	public void BuyUpgrade(Upgrade upgrade)
	{
		score -= upgrade.baseCurrencyCost;
		currentGeneration += upgrade.generation;
		currentEnergy += upgrade.energyUsage;
		currentEnergy = Math.Max(currentEnergy - currentEnergy * upgrade.energyConsumptionDecrease, 0);
		
		ui.UpdateEnergyDisplay(currentEnergy, maxEnergy);
		if(currentEnergy >= maxEnergy)
		{
			ui.UpdateEnergyDisplayDanger(true);
		}
		else
		{
			ui.UpdateEnergyDisplayDanger(false);
		}

		if (!upgradeCounts.ContainsKey(upgrade.id))
		{
			upgradeCounts.Add(upgrade.id, 0);
		}
		upgradeCounts[upgrade.id]++;

		OnUpgradeBought(upgrade);
	}
}
