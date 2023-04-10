using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
	// Anything saved in `data` and `settings` is persistent
	public GameData data;
	public GameSettings settings;

	private static UIManager ui;
	public List<Upgrade> unlockedUpgrades;
	public List<ResearchNode> unlockedResearches;
	public ResearchNode activeResearch;

	private static readonly float baseMaxEnergy = 100;

	[ReadOnly]
	public float maxEnergy = baseMaxEnergy;
	[ReadOnly]
	public float researchProduction = 0;
	// [ReadOnly] // Doesn't work in conjuction with HugeNumber :(
	public HugeNumber currencyGeneration = new HugeNumber(0);
	[ReadOnly]
	public float rawEnergyUsage = 0;
	[ReadOnly]
	public float energyUsageEfficiency = 0;

	public HugeNumber currencyPerClick = new HugeNumber(1);

	public static event Action<Upgrade> OnUpgradeUnlocked;
	public static event Action<ResearchNode> OnResearchStarted;
	public static event Action<ResearchNode> OnResearchFinished;
	public static event Action<ResearchNode> OnResearchStopped;

	public override void Setup()
	{
		settings = new GameSettings();
		data = new GameData();
		ui = GameObject.Find("/UI").GetComponent<UIManager>();

		unlockedResearches = new List<ResearchNode>();
		unlockedUpgrades = new List<Upgrade>();
		LoadSaveFile();
	}

	void Update()
	{
		if(SceneManager.GetActiveScene().buildIndex == 0) return; // if in main menu do nothing

		data.currency += currencyGeneration * Time.deltaTime;

		if (activeResearch)
		{
			data.researchProgress += researchProduction * Time.deltaTime;
			float percent = Math.Clamp(data.researchProgress / activeResearch.researchCost, 0, 1);
			ui.UpdateResearchProgress(percent);

			if (data.researchProgress >= activeResearch.researchCost)
			{
				ResearchFinished();
			}
		}

		ui.UpdateScoreDisplay(data.currency);
	}

	public void GenerateCurrency()
	{
		data.currency += currencyPerClick;
	}

	public static void RestartScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public float GetEnergyUsage()
	{
		return rawEnergyUsage * Math.Clamp(energyUsageEfficiency, 0, 1);
	}

	// ======================= Saving/Loading ========================

	public void LoadSaveFile()
	{
		SaveObject save = SaveManager.Load();
		if (save == null) return;

		settings = save.settings;
		data = save.data;

		// Apply research
		var allResearch = Resources.FindObjectsOfTypeAll<ResearchNode>();
		var unlockedResearch = data.unlockedResearch;
		data.unlockedResearch = new List<string>();
		foreach (var id in unlockedResearch)
		{
			var research = allResearch.Where(r => r.id == id).First();
			if (research == null)
			{
				Debug.LogWarning($"Tried to load research with unknown id: {id}");
				continue;
			}

			foreach (var upgrade in research.unlockUpgrades)
			{
				UnlockUpgrade(upgrade);
			}
			unlockedResearches.Add(research);
			data.unlockedResearch.Add(id);
		}

		// Apply current research
		if (data.activeResearch != null)
		{
			activeResearch = allResearch.Where(research => research.id == data.activeResearch).First();
			if (activeResearch == null)
			{
				Debug.LogWarning($"Tried to start research with unknown id: {data.activeResearch}");
				data.activeResearch = null;
			}
		}

		// Recalculate effects of research and upgrades
		RefreshUpgradeAndResearchEffects();
	}

	public SaveObject GetSaveObject()
	{
		SaveObject save = new SaveObject();
		save.settings = settings;
		save.data = data;
		return save;
	}

	// ======================= Upgrades ========================

	public void UnlockUpgrade(Upgrade upgrade)
	{
		if (unlockedUpgrades.Contains(upgrade)) return;

		unlockedUpgrades.Add(upgrade);
		OnUpgradeUnlocked?.Invoke(upgrade);
	}

	public void BuyUpgrade(Upgrade upgrade)
	{
		// Stop, if not enough currency
		if (upgrade.baseCurrencyCost > data.currency) return;

		data.currency -= upgrade.baseCurrencyCost;

		if (!data.upgradeCounts.ContainsKey(upgrade.id))
		{
			data.upgradeCounts.Add(upgrade.id, 0);
		}
		data.upgradeCounts[upgrade.id]++;

		RefreshUpgradeAndResearchEffects();

		ui.UpdateResearchSpeedDisplay(researchProduction);
		ui.UpdateEnergyDisplay(GetEnergyUsage(), maxEnergy);
		ui.UpdateEnergyDisplayDanger(GetEnergyUsage() >= maxEnergy);		
	}

	public void DisableUpgrade(Upgrade upgrade)
	{
		data.disabledUpgrades.Add(upgrade.id);
		RefreshUpgradeAndResearchEffects();

		ui.UpdateResearchSpeedDisplay(researchProduction);
		ui.UpdateEnergyDisplay(GetEnergyUsage(), maxEnergy);
		ui.UpdateEnergyDisplayDanger(GetEnergyUsage() >= maxEnergy);
	}

	public void EnableUpgrade(Upgrade upgrade)
	{
		data.disabledUpgrades.Remove(upgrade.id);
		RefreshUpgradeAndResearchEffects();

		ui.UpdateResearchSpeedDisplay(researchProduction);
		ui.UpdateEnergyDisplay(GetEnergyUsage(), maxEnergy);
		ui.UpdateEnergyDisplayDanger(GetEnergyUsage() >= maxEnergy);
	}

	private void ApplyUpgradeEffects(Upgrade upgrade, int count)
	{
		currencyGeneration    += upgrade.currencyGeneration * count;
		rawEnergyUsage        += upgrade.energyUsage        * count;
		researchProduction    += upgrade.researchProduction * count;
		maxEnergy             += upgrade.energyCapRaise     * count;
		energyUsageEfficiency *= (float)Math.Pow(1 - upgrade.energyConsumptionDecrease, count);
	}

	// When modifying this function, keep in mind that this is called every time
	// a new upgrade is bought, disabled, enabled or even research finished.
	//
	// So make it that calling `RefreshUpgradeAndResearchEffects()` multiple times in a row,
	// is the same as calling it only once.
	private void RefreshUpgradeAndResearchEffects()
	{
		currencyGeneration = new HugeNumber(0);
		rawEnergyUsage = 0;
		energyUsageEfficiency = 1;
		researchProduction = 0;
		maxEnergy = baseMaxEnergy;

		foreach (var upgrade in unlockedUpgrades)
		{
			if (data.disabledUpgrades.Contains(upgrade.id)) continue;

			var count = data.upgradeCounts.GetValueOrDefault(upgrade.id, 0);
			ApplyUpgradeEffects(upgrade, count);
		}

		foreach (var research in unlockedResearches)
		{
			if (!research.instantUnlock) continue;

			foreach (var upgrade in research.unlockUpgrades)
			{
				ApplyUpgradeEffects(upgrade, 1);
			}
		}

		// TODO: update ui
		// ui.UpdateResearchSpeedDisplay(researchProduction);
		// ui.UpdateEnergyDisplay(GetEnergyUsage(), maxEnergy);
		// ui.UpdateEnergyDisplayDanger(energyUsage >= maxEnergy);
	}

	// ======================= Research ========================

	public bool StartResearch(ResearchNode research)
	{
		// TODO: Check if player has unlocked at least 1 previous research, check if player has dependency
		if (unlockedResearches.Contains(activeResearch)) return false;
		if (research.currencyCost > data.currency)
		{
			return false;
		}
		if (activeResearch)
		{
			OnResearchStopped(activeResearch);
		}
		data.currency -= research.currencyCost;

		activeResearch = research;
		data.activeResearch = research.id;
		if (research.researchCost > 0)
		{
			data.researchProgress = 0;
			ui.UpdateCurrentResearchLabel(research.displayName);
			OnResearchStarted(research);
		}
		else
		{
			OnResearchStarted(research);
			ResearchFinished();
		}

		return true;
	}

	private void ResetActiveResearch()
	{
		data.researchProgress = 0;
		activeResearch = null;
		data.activeResearch = null;

		ui.UpdateResearchProgress(0);
		ui.UpdateCurrentResearchLabel("");
	}

	public void StopResearch()
	{
		if (activeResearch == null) return;

		var research = activeResearch;
		ResetActiveResearch();
		OnResearchStopped(research);
	}

	private void ResearchFinished()
	{
		foreach (var upgrade in activeResearch.unlockUpgrades)
		{
			UnlockUpgrade(upgrade);
		}
		unlockedResearches.Add(activeResearch);
		data.unlockedResearch.Add(activeResearch.id);

		var research = activeResearch;
		ResetActiveResearch();
		OnResearchFinished(research);
	}
}