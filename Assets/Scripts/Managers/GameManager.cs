using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

//store settings
[System.Serializable]
public class GameSettings
{
	public int windowState;
	public string windowStateName;
	public float volumeLevel;
	public  GameSettings()
	{
		windowState = 1;
		windowStateName = "Large";
		volumeLevel = 50f;
	}
}
[System.Serializable]
public class GameData
{
	public DateTime lastSave;
	public HugeNumber currency;

	public Dictionary<string, int> upgradeCounts;

	//[ReadOnly]
	public float researchProduction;
	//[ReadOnly]
	public HugeNumber currencyGeneration;
	//[ReadOnly]
	public float energyUsage;
	
	public float maxEnergy;

	public HugeNumber clickMultiplier;

	public List<string> unlockedUpgrade;
	public List<string> unlockedResearch;
	public GameData()
	{
		lastSave = DateTime.MinValue;
		currencyGeneration = new HugeNumber(0);
		researchProduction = 0;
		currency = new HugeNumber(0);
		energyUsage = 0;
		maxEnergy = 0;
		clickMultiplier = new HugeNumber(1);
		unlockedResearch = new List<string>();
		unlockedUpgrade = new List<string>();
		upgradeCounts = new Dictionary<string, int>();
	}
	/// <summary>
	/// Gets list of paths of scriptable objects assets (cant serialize scriptable objects themselves)
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="objects"></param>
	/// <param name="path"></param>
	/// <returns></returns>
	public List<string> GetScriptableObjectPaths<T>(List<T> objects, string path) where T: ScriptableObject
	{
		List<string> paths = new List<string>();
		foreach(var asset in objects) 
		{
			string fullPath = path + "/" + asset.name;
			paths.Add(fullPath);
		}
		return paths;
	}
	/// <summary>
	/// Loads scriptable objects from given paths list
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="objectPaths"></param>
	/// <returns></returns>
	public List<T> LoadScriptableObjects<T>(List<string> objectPaths) where T : ScriptableObject
	{
		List<T> objects = new List<T>();
		foreach (string path in objectPaths)
		{
			objects.Add((T)Resources.Load(path));
		}
		return objects;
	}
}

public class GameManager : MonoBehaviour
{
	public GameData data;
	public GameSettings settings;
	public static GameManager instance;
	private static UIManager ui;
	public List<Upgrade> allUpgrades;
	public List<Upgrade> unlockedUpgradeObjs;
	public List<ResearchNode> unlockedResearchObjs;
	
	private ResearchNode activeResearch;
	private float researchProgress;

	public static event Action<Upgrade> OnUpgradeBought;
	public static event Action<ResearchNode> OnResearchStarted;
	public static event Action<ResearchNode> OnResearchFinished;
	public static event Action<ResearchNode> OnResearchStopped;


	// GAME SETTINGS
	public string saveFile = "clicky.sav"; 
	public string savePath;


	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(this);
			return;
		}
		instance = this;
		Debug.Log("game manager start");
		//DontDestroyOnLoad(this.gameObject);
		Setup();
	}

	void Setup()
	{
		settings = new GameSettings();
		data = new GameData();
		ui = GameObject.Find("/UI").GetComponent<UIManager>();
		savePath = Path.Combine(Application.persistentDataPath, saveFile);
		LoadData();
	}

	void Update()
	{
		if(SceneManager.GetActiveScene().buildIndex == 0) return; // if in main menu do nothing 

		data.currency += data.currencyGeneration * Time.deltaTime;

		if (activeResearch)
		{
			researchProgress += data.researchProduction * Time.deltaTime;
			float percent = Math.Clamp(researchProgress / activeResearch.researchCost, 0, 1);
			ui.UpdateResearchProgress(percent);

			if (researchProgress >= activeResearch.researchCost)
			{
				ResearchFinished();
			}
		}

		ui.UpdateScoreDisplay(data.currency);
	}

	public void GenerateCurrency()
	{
		data.currency += data.clickMultiplier;
	}

	public void RestartScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void UnlockUpgrade(Upgrade upgrade)
	{
		if (unlockedUpgradeObjs.Contains(upgrade)) return;

		unlockedUpgradeObjs.Add(upgrade);
		ui.AppendUpgradeButton(upgrade);
	}

	public void BuyUpgrade(Upgrade upgrade)
	{
		data.currency -= upgrade.baseCurrencyCost;
		data.currencyGeneration += upgrade.currencyGeneration;
		data.energyUsage += upgrade.energyUsage;
		data.energyUsage = Math.Max(data.energyUsage - data.energyUsage * upgrade.energyConsumptionDecrease, 0);

		data.researchProduction += upgrade.researchProduction;
		ui.UpdateResearchSpeedDisplay(data.researchProduction);
		
		ui.UpdateEnergyDisplay(data.energyUsage, data.maxEnergy);
		if(data.energyUsage >= data.maxEnergy)
		{
			ui.UpdateEnergyDisplayDanger(true);
		}
		else
		{
			ui.UpdateEnergyDisplayDanger(false);
		}

		if (!data.upgradeCounts.ContainsKey(upgrade.id))
		{
			data.upgradeCounts.Add(upgrade.id, 0);
		}
		data.upgradeCounts[upgrade.id]++;

		OnUpgradeBought(upgrade);
	}

	public void DisableUpgrade(Upgrade upgrade)
	{
		if (!data.upgradeCounts.ContainsKey(upgrade.id))
		{
			data.upgradeCounts.Add(upgrade.id, 0);
		}

		data.currencyGeneration -= upgrade.currencyGeneration * data.upgradeCounts[upgrade.id];
		data.energyUsage -= upgrade.energyUsage * data.upgradeCounts[upgrade.id];
		data.energyUsage = Math.Max(data.energyUsage - data.energyUsage * upgrade.energyConsumptionDecrease, 0);
		data.researchProduction -= upgrade.researchProduction * data.upgradeCounts[upgrade.id];
		ui.UpdateResearchSpeedDisplay(data.researchProduction);
		ui.UpdateEnergyDisplay(data.energyUsage, data.maxEnergy);

		if (data.energyUsage >= data.maxEnergy)
		{
			ui.UpdateEnergyDisplayDanger(true);
		}
		else
		{
			ui.UpdateEnergyDisplayDanger(false);
		}
	}

	public void EnableUpgrade(Upgrade upgrade)
	{
		if (!data.upgradeCounts.ContainsKey(upgrade.id))
		{
			data.upgradeCounts.Add(upgrade.id, 0);
		}

		data.currencyGeneration += upgrade.currencyGeneration * data.upgradeCounts[upgrade.id];
		data.energyUsage += upgrade.energyUsage * data.upgradeCounts[upgrade.id];
		data.energyUsage = Math.Max(data.energyUsage - data.energyUsage * upgrade.energyConsumptionDecrease, 0);
		data.researchProduction += upgrade.researchProduction * data.upgradeCounts[upgrade.id];
		ui.UpdateResearchSpeedDisplay(data.researchProduction);
		ui.UpdateEnergyDisplay(data.energyUsage, data.maxEnergy);

		if (data.energyUsage >= data.maxEnergy)
		{
			ui.UpdateEnergyDisplayDanger(true);
		}
		else
		{
			ui.UpdateEnergyDisplayDanger(false);
		}
	}

	public bool StartResearch(ResearchNode research)
	{
		
		// TODO: Check if player has unlocked at least 1 previous research
		if (unlockedResearchObjs.Contains(activeResearch)) return false;
		if (research.currencyCost > data.currency)
		{
			return false;
		}
		if(activeResearch)
		{
			OnResearchStopped(activeResearch);
		}
		data.currency -= research.currencyCost;
		
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
		
		unlockedResearchObjs.Add(activeResearch);
		if(!activeResearch.instantUnlock)
		{
			foreach (var upgrade in activeResearch.unlockUpgrades)
			{
				UnlockUpgrade(upgrade);
			}
		}
		else
		{
			ApplyInstantUpgrades(activeResearch.unlockUpgrades);
		}

		var research = activeResearch;
		StopResearchWithoutEvent();
		OnResearchFinished(research);
	}
	public void ApplyInstantUpgrades(List<Upgrade> upgrades)
	{
		foreach(Upgrade upgrade in upgrades)
		{
			data.currencyGeneration += upgrade.currencyGeneration;
			data.maxEnergy += upgrade.energyCapRaise;
			data.researchProduction += upgrade.researchProduction;
			data.energyUsage = Math.Max(data.energyUsage - data.energyUsage * upgrade.energyConsumptionDecrease, 0);
		}
		ui.UpdateResearchSpeedDisplay(data.researchProduction);

		ui.UpdateEnergyDisplay(data.energyUsage,data. maxEnergy);
	}

	public void LoadData()
	{
		SaveObject save = SaveManager.Load(savePath);
		if (save == null) return;
		SetData(save);
		
	}

	/// <summary>
	/// Sets all the data from a save object 
	/// </summary>
	/// <param name="save"></param>
	public void SetData(SaveObject save)
	{
		if (save == null) return;
		settings = save.settings;
		data = save.data;
		unlockedResearchObjs = data.LoadScriptableObjects<ResearchNode>(data.unlockedResearch);
		unlockedUpgradeObjs = data.LoadScriptableObjects<Upgrade>(data.unlockedUpgrade);
	}

	/// <summary>
	/// Creates a save object from data
	/// </summary>
	/// <returns></returns>
	public SaveObject GetData()
	{
		SaveObject save = new SaveObject();
		data.unlockedResearch = data.GetScriptableObjectPaths<ResearchNode>(unlockedResearchObjs, "Research");
		data.unlockedUpgrade = data.GetScriptableObjectPaths<Upgrade>(unlockedUpgradeObjs, "Upgrades");
		data.lastSave = DateTime.Now;
		save.settings = settings;
		save.data = data;
		return save;
	}
}
