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

	public float currency = 0;

	public Dictionary<string, int> upgradeCounts;

	[ReadOnly]
	public float researchProduction = 0;
	[ReadOnly]
	public float currencyGeneration = 0;
	[ReadOnly]
	public float energyUsage = 0;
	
	public float maxEnergy = 1000; // TOD: Make this upgradable

	public float clickMultiplier = 1;

	public List<Upgrade> unlockedUpgrades;
	public List<ResearchNode> unlockedResearch;

	private ResearchNode activeResearch;
	private float researchProgress;

	public static event Action<Upgrade> OnUpgradeBought;
	public static event Action<ResearchNode> OnResearchStarted;
	public static event Action<ResearchNode> OnResearchFinished;
	public static event Action<ResearchNode> OnResearchStopped;

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
		ui.UpdateEnergyDisplay(energyUsage, maxEnergy);
		ui.UpdateResearchSpeedDisplay(researchProduction);
		//foreach (var upgrade in Resources.LoadAll<Upgrade>("Upgrades"))
		//{
		//	UnlockUpgrade(upgrade);
		//}
	}

	void Update()
	{
		currency += currencyGeneration * Time.deltaTime;

		if (activeResearch)
		{
			researchProgress += researchProduction * Time.deltaTime;
			float percent = Math.Clamp(researchProgress / activeResearch.researchCost, 0, 1);
			ui.UpdateResearchProgress(percent);

			if (researchProgress >= activeResearch.researchCost)
			{
				ResearchFinished();
			}
		}

		ui.UpdateScoreDisplay((ulong)currency);
	}

	public void GenerateCurrency()
	{
		currency += clickMultiplier;
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
		currency -= upgrade.baseCurrencyCost;
		currencyGeneration += upgrade.currencyGeneration;
		energyUsage += upgrade.energyUsage;
		energyUsage = Math.Max(energyUsage - energyUsage * upgrade.energyConsumptionDecrease, 0);

		researchProduction += upgrade.researchProduction;
		ui.UpdateResearchSpeedDisplay(researchProduction);
		
		ui.UpdateEnergyDisplay(energyUsage, maxEnergy);
		if(energyUsage >= maxEnergy)
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

	public bool StartResearch(ResearchNode research)
	{
		// TODO: Check if player has unlocked at least 1 previous research
		if (unlockedResearch.Contains(activeResearch)) return false;
		if (research.currencyCost > currency)
		{
			return false;
		}
		if(activeResearch)
		{
			OnResearchStopped(activeResearch);
		}
		currency -= research.currencyCost;
		
		activeResearch = research;
		if (research.researchCost > 0)
		{
			researchProgress = 0;
			ui.UpdateCurrentResearchLabel(research.displayName);
			OnResearchStarted(research);
		}  else
		{
			OnResearchStarted(research);
			ResearchFinished();
		}

		return true;
	}

	private void StopResearchWithoutEvent()
	{
		researchProgress = 0;
		ui.UpdateResearchProgress(0);
		ui.UpdateCurrentResearchLabel("");
		activeResearch = null;
	}

	public void StopResearch()
	{
		var research = activeResearch;
		StopResearchWithoutEvent();
		OnResearchStopped(research);
	}

	public void ResearchFinished()
	{
		unlockedResearch.Add(activeResearch);

		foreach (var upgrade in activeResearch.unlockUpgrades)
		{
			UnlockUpgrade(upgrade);
		}

		var research = activeResearch;
		StopResearchWithoutEvent();
		OnResearchFinished(research);
	}
}
