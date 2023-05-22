using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	// Anything saved in `data` and `settings` is persistent
	public GameData data;
	public GameSettings settings;

	private static UIManager ui;
	public List<Upgrade> unlockedUpgrades;
	public List<ResearchNode> unlockedResearches;
	public ResearchNode activeResearch;
	public List<Upgrade> upgradesInShop;

	[ReadOnly]
	public float baseMaxEnergy = 200;
	
	public float maxEnergy = 0;
	
	public float researchProduction = 0;
	// [ReadOnly] // Doesn't work in conjuction with HugeNumber :(
	public HugeNumber currencyGeneration = new HugeNumber(0);

	[ReadOnly]
	public float rawEnergyUsage = 0;
	[ReadOnly]
	public float energyUsageEfficiency = 0;
	
	public HugeNumber currencyPerClick = new HugeNumber(1);

	public float researchPercent = 0.0f;

	public bool startCriticalEnergy = false;
	public float currentTime;
	float baseTime;
	public static event Action<Upgrade> OnUpgradeUnlocked;
	public static event Action<ResearchNode> OnResearchStarted;
	public static event Action<ResearchNode> OnResearchFinished;
	public static event Action<ResearchNode> OnResearchStopped;

	public AudioClip researchSound;

	public void Awake()
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
		settings = new GameSettings();
		data = new GameData();
		ui = GameObject.Find("/UI").GetComponent<UIManager>();

		unlockedResearches = new List<ResearchNode>();
		unlockedUpgrades = new List<Upgrade>();
		upgradesInShop = new List<Upgrade>();
		
		CheckResourcesIds();
		LoadSaveFile();

		Time.timeScale = 1f;

		maxEnergy = baseMaxEnergy;
	}

	//cant use generics with scriptable objects ??
	public void UnsubsribeEvents()
	{
		foreach(var d in OnUpgradeUnlocked.GetInvocationList())
		{
			OnUpgradeUnlocked -= (Action<Upgrade>)d;
		}

		foreach(var d in OnResearchStarted.GetInvocationList())
		{
			OnResearchStarted -= (Action<ResearchNode>)d;
		}

		foreach(var d in OnResearchStopped.GetInvocationList())
		{
			OnResearchStopped -= (Action<ResearchNode>)d;
		}

		foreach(var d in OnResearchFinished.GetInvocationList())
		{
			OnResearchFinished -= (Action<ResearchNode>)d;
		}
	}
	void Update()
	{
		if(SceneManager.GetActiveScene().buildIndex == 0) return; // if in main menu do nothing

		if(!startCriticalEnergy)
		{
			data.currency += currencyGeneration * Time.deltaTime * EventManager.instance.generationMultiplier;
		}
			 

		if (activeResearch)
		{
			float multiplier = EventManager.instance.researchMultiplier;
			data.researchProgress += researchProduction * Time.deltaTime * multiplier;
			researchPercent = Math.Clamp(data.researchProgress / activeResearch.researchCost, 0, 1);

			if (data.researchProgress >= activeResearch.researchCost)
			{
				researchPercent = 0.0f;
				ResearchFinished(activeResearch);
				ResetActiveResearch();
			}
		}
		
		CheckEnergy();

		ui.UpdateScoreDisplay(data.currency);
	}

	public void GenerateCurrency()
	{
		data.currency += currencyPerClick * EventManager.instance.clickMultiplier;
	}

	public  void RestartGame()
	{
		SaveManager.DeleteSave();
		UnsubsribeEvents();
		SceneManager.LoadScene(0);
	}
	public  void ExitMainMenu()
	{
		UnsubsribeEvents();
		SceneManager.LoadScene(0);
	}

	public float GetEnergyUsage()
	{
		return rawEnergyUsage * Math.Clamp(energyUsageEfficiency, 0, 1);
	}

	public void CheckResourcesIds()
	{
		// Check to make sure each research has a unique id
		var allResearch = Resources.FindObjectsOfTypeAll<ResearchNode>();
		foreach (var group in allResearch.GroupBy(r => r.id))
		{
			if (group.Count() > 1)
			{
				var names = string.Join(", ", group.Select(r => r.name));
				Debug.LogWarning($"Research id '{group.Key}' is used between: {names}");
			}
		}

		var allUpgrades = Resources.FindObjectsOfTypeAll<Upgrade>();
		foreach (var group in allUpgrades.GroupBy(r => r.id))
		{
			if (group.Count() > 1)
			{
				var names = string.Join(", ", group.Select(r => r.name));
				Debug.LogWarning($"Upgrade id '{group.Key}' is used between: {names}");
			}
		}
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
			var research = allResearch.Where(r => r.id == id).FirstOrDefault();
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
		if (data.activeResearch.NullIfEmpty() != null)
		{
			activeResearch = allResearch.Where(research => research.id == data.activeResearch).FirstOrDefault();
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
		if(upgrade.instantUnlock)
		{
			data.upgradeCounts.Add(upgrade.id, 1);
			RefreshUpgradeAndResearchEffects();
		}
		else upgradesInShop.Add(upgrade);
			
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
	}

	public void DisableUpgrade(Upgrade upgrade)
	{
		data.disabledUpgrades.Add(upgrade.id);
		RefreshUpgradeAndResearchEffects();

		ui.UpdateResearchSpeedDisplay(researchProduction);
	}

	public void EnableUpgrade(Upgrade upgrade)
	{
		data.disabledUpgrades.Remove(upgrade.id);
		RefreshUpgradeAndResearchEffects();

		ui.UpdateResearchSpeedDisplay(researchProduction);
	}

	private void ApplyUpgradeEffects(Upgrade upgrade, int count)
	{
		currencyPerClick += upgrade.currencyPerClick * count;
		currencyGeneration    += upgrade.currencyGeneration * count;
		rawEnergyUsage        += upgrade.energyUsage        * count;
		researchProduction    += upgrade.researchProduction * count;
		baseMaxEnergy             += upgrade.energyCapRaise     * count;
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
		currencyPerClick = new HugeNumber(1);
		rawEnergyUsage = 0;
		energyUsageEfficiency = 1;
		researchProduction = 0;
		baseMaxEnergy = 200; // :(
		foreach (var upgrade in unlockedUpgrades)
		{
			if (data.disabledUpgrades.Contains(upgrade.id)) continue;

			var count = data.upgradeCounts.GetValueOrDefault(upgrade.id, 0);
			ApplyUpgradeEffects(upgrade, count);
		}
		maxEnergy = baseMaxEnergy;
	

		// TODO: update ui
		// ui.UpdateResearchSpeedDisplay(researchProduction);
		// ui.UpdateEnergyDisplay(GetEnergyUsage(), maxEnergy);
		// ui.UpdateEnergyDisplayDanger(energyUsage >= maxEnergy);
	}

	// ======================= Research ========================

	public void UnlockResearchForFreeNow(ResearchNode research)
	{		
		if (unlockedResearches.Contains(research)) return;
		OnResearchStarted?.Invoke(research);
		ResearchFinished(research);
	}

	public bool StartResearch(ResearchNode research)
	{
		// TODO: Check if player has unlocked at least 1 previous research, check if player has dependency
		if (unlockedResearches.Contains(research)) return false;
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
			OnResearchStarted?.Invoke(research);
		}
		else
		{
			OnResearchStarted?.Invoke(research);
			ResearchFinished(research);
			ResetActiveResearch();
		}

		return true;
	}

	private void ResetActiveResearch()
	{
		data.researchProgress = 0;
		activeResearch = null;
		data.activeResearch = null;
	}

	public void StopResearch()
	{
		if (activeResearch == null) return;

		var research = activeResearch;
		ResetActiveResearch();
		OnResearchStopped?.Invoke(research);
	}

	private void ResearchFinished(ResearchNode research)
	{
		AudioManager.PlaySingleSound(researchSound, 0.5f);
		foreach (var upgrade in research.unlockUpgrades)
		{
			UnlockUpgrade(upgrade);
		}
		unlockedResearches.Add(research);
		data.unlockedResearch.Add(research.id);

		OnResearchFinished?.Invoke(research);
	}
	/// <summary>
	/// Constantly checks energy usage level, if it reaches max cap, decrease currency
	/// Every ~5 seconds, currency decrease becomes larger
	/// Upon restoring safe levels, refresh stats and continue as normal
	/// </summary>
	void CheckEnergy()
	{
		if (GetEnergyUsage() >= maxEnergy && !startCriticalEnergy)
		{
			startCriticalEnergy = true;
			currentTime = 0;
		}

		if(startCriticalEnergy)
		{
			CheckCurrency();
			currentTime += Time.deltaTime;
			
			data.currency -= (currencyGeneration + (int)Math.Pow(1.2,currentTime)) * Time.deltaTime * (currencyGeneration.value == 0 ? 1 : (float)Math.Ceiling(currencyGeneration.value / 10)); // goofy aah scaling
			if (GetEnergyUsage() < maxEnergy)
			{
				startCriticalEnergy = false;
				RefreshUpgradeAndResearchEffects();
			}
		}
	}
	//
	void CheckCurrency()
	{
		if(data.currency <= new HugeNumber(0)) //gg
		{
			data.currency = new HugeNumber(0);
			ui.SetGameOverShown(true);
			data = new GameData();
			SaveManager.Save(GetSaveObject());
			Time.timeScale = 0f;
		}
	}
}