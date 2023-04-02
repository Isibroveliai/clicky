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
	public GameSettings()
	{
		windowState = 1;
		windowStateName = "Large";
		volumeLevel = 100f; //rip ears
	}
	// public GameSettings(int windowState, string windowStateName)
	// {
	// 	this.windowState = windowState;
	// 	this.windowStateName = windowStateName;
	// }
}
//PASIULYMAS: PERKELT DATA I ATSKIRA KLASE?
//public class GameData{}

public class GameManager : MonoBehaviour
{
	public GameSettings settings;
	public static GameManager instance;
	private static UIManager ui;
	public List<Upgrade> allUpgrades;

	public float currency = 0;

	public Dictionary<string, int> upgradeCounts;

	//[ReadOnly]
	public float researchProduction = 0;
	//[ReadOnly]
	public float currencyGeneration = 0;
	//[ReadOnly]
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


	// GAME SETTINGS
	public string saveFile = "clicky.sav"; 
	public string savePath;
	public DateTime lastSave;

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
		//DontDestroyOnLoad(this.gameObject);
		Setup();
	}

	void Setup()
	{
		ui = GameObject.Find("/UI").GetComponent<UIManager>();
		ui.UpdateUpgradeDescription("");
		ui.UpdateEnergyDisplay(energyUsage, maxEnergy);
		ui.UpdateResearchSpeedDisplay(researchProduction);
		ui.UpdateSaveInfoText("");
		savePath = Path.Combine(Application.persistentDataPath, saveFile);
		settings = new GameSettings();
		LoadData();
		ui.UpdateWindowChangeButtonText(settings.windowStateName);
		ui.SetVolumeValue(settings.volumeLevel);
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
	public void IncrementWindowState()
	{
		settings.windowState++;
		if (settings.windowState >= 2)
		{
			settings.windowState = 0;
		}
		ChangeWindowSize(settings.windowState);
	}
	public void ChangeWindowSize(int state)
	{
		switch (state)
		{
			case 0:
				settings.windowStateName = "Small";
				Screen.SetResolution(640, 480, false);
				break;

			case 1:
				settings.windowStateName = "Large";
				Screen.SetResolution(1280, 960, false);
				break;
			// maybe in future idk, looks shit now
			// case 2:
			// 	settings.windowStateName = "Fullscreen";
				
			// 	Screen.SetResolution(1280, 960, true);

			// 	break;

			default:
				break;
		}
		ui.UpdateWindowChangeButtonText(settings.windowStateName);
	}
	public void QuitGame()
	{
		SaveGame();
		Application.Quit();
	}
	public void SaveGame()
	{
		SaveManager.Save(GetData(), savePath);
		ui.UpdateSaveInfoText(DateTime.Now.ToString("yyyy-dd-MM HH:mm:ss"));
	}

	public void LoadData()
	{
		SaveObject save = SaveManager.Load(savePath);
		if (save == null) return;
		SetData(save);
		ui.UpdateSaveInfoText(lastSave.ToString("yyyy-dd-MM HH:mm:ss"));
	}
	public void DeleteSave()
	{
		if(File.Exists(savePath))
		{
			File.Delete(savePath);
			Debug.Log("Save deleted");
			return;
		}
		Debug.Log("Save file not found");
	}
	/// <summary>
	/// Sets all the data from a save object 
	/// </summary>
	/// <param name="save"></param>
	public void SetData(SaveObject save)
	{
		if (save == null) return;
		clickMultiplier = save.clickMultiplier;
		currency = save.currency;
		currencyGeneration = save.currencyGeneration;
		researchProduction = save.researchProduction;
		energyUsage = save.energyUsage;
		maxEnergy = save.maxEnergy;
		
		unlockedResearch = LoadScriptableObjects<ResearchNode>(save.unlockedResearch);
		unlockedUpgrades = LoadScriptableObjects<Upgrade>(save.unlockedUpgrades);
		upgradeCounts = save.upgradeCounts;
		settings = save.settings;
		lastSave = save.saveTime;
	}
	/// <summary>
	/// Creates a save object from data
	/// </summary>
	/// <returns></returns>
	public SaveObject GetData()
	{
		SaveObject save = new SaveObject();
		save.saveTime = DateTime.Now;
		save.clickMultiplier = clickMultiplier;
		save.currency = currency;
		save.currencyGeneration = currencyGeneration;
		save.researchProduction = researchProduction;
		save.energyUsage = energyUsage;
		save.maxEnergy = maxEnergy;
		save.unlockedResearch = GetScriptableObjectPaths(unlockedResearch, "Research");
		save.unlockedUpgrades = GetScriptableObjectPaths(unlockedUpgrades, "Upgrades");
		save.upgradeCounts = upgradeCounts;
		save.settings = settings;

		return save;
	}
	/// <summary>
	/// Gets list of paths of scriptable objects assets (cant serialize scriptable objects themselves)
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="objects"></param>
	/// <param name="path"></param>
	/// <returns></returns>
	List<string> GetScriptableObjectPaths<T>(List<T> objects, string path) where T: ScriptableObject
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
	List<T> LoadScriptableObjects<T>(List<string> objectPaths) where T : ScriptableObject
	{
		List<T> objects = new List<T>();
		foreach (string path in objectPaths)
		{
			objects.Add((T)Resources.Load(path));
		}
		return objects;
	}
}
