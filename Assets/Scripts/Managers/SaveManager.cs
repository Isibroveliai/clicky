using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;

public class SaveManager : MonoBehaviour
{
	GameManager gm;
	UIManager ui;

	string file = "clicky.sav";
	string dir;
	string path;
    void Awake()
    {
		gm = GameManager.instance;
		
    }

	public void Save(SaveObject save)
	{
		dir = Application.persistentDataPath + "/" + "Clicky";
		path = Path.Combine(dir, file);
		if (!Directory.Exists(dir))
		{
			Directory.CreateDirectory(dir);
		}
		FileStream fs = new FileStream(path, FileMode.Create);
		
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(fs, save);
		fs.Close();
		Debug.Log("Saved data");
		Debug.Log(path);
		
	}
	public SaveObject Load()
	{
		dir = Application.persistentDataPath + "/" + "Clicky";
		path = Path.Combine(dir, file);
		if (!File.Exists(path)) 
		{
			Debug.Log("No save file detected");
			return null;
		}

		try
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream fs = new FileStream(path,FileMode.Open);
			SaveObject save = (SaveObject)bf.Deserialize(fs);
			fs.Close();
			return save;

		}
		catch (SerializationException)
		{
			Debug.Log("Failed to deserialize");
		}
		return null;
	}

	//TODO: maybe place these in game manager 
	////ugly methods to set/get data 
	public void SetData(SaveObject save)
	{
		if (save == null) return;
		gm.clickMultiplier = save.clickMultiplier;
		gm.currency = save.currency;
		gm.currencyGeneration = save.currencyGeneration;
		gm.researchProduction = save.researchProduction;
		gm.energyUsage = save.energyUsage;
		gm.maxEnergy = save.maxEnergy;
		List<ResearchNode> research = new List<ResearchNode>();
		List<Upgrade> upgrades = new List<Upgrade>();
		foreach(string path in save.unlockedUpgrades)
		{
			upgrades.Add((Upgrade)Resources.Load(path));
		}
		foreach (string path in save.unlockedResearch)
		{
			research.Add((ResearchNode)Resources.Load(path));
		}
		gm.unlockedResearch = research;
		gm.unlockedUpgrades = upgrades;
		gm.upgradeCounts = save.upgradeCounts;
	}
	public SaveObject GetData()
	{
		SaveObject save = new SaveObject();
		save.clickMultiplier = gm.clickMultiplier;
		save.currency = gm.currency;
		save.currencyGeneration = gm.currencyGeneration;
		save.researchProduction = gm.researchProduction;
		save.energyUsage = gm.energyUsage;
		save.maxEnergy = gm.maxEnergy;

		List<string> researchPaths = new List<string>();
		foreach (var asset in gm.unlockedResearch)
		{
			if (asset == null) continue;
			string path = "Research/" + asset.name;
			researchPaths.Add(path);
		}
		List<string> upgradePaths = new List<string>();
		foreach (var asset in gm.unlockedUpgrades)
		{
			if (asset == null) continue;
			string path = "Upgrades/" + asset.name;
			upgradePaths.Add(path);
		}
		save.unlockedResearch = researchPaths;
		save.unlockedUpgrades = upgradePaths;
		save.upgradeCounts = gm.upgradeCounts;
		return save;
	}

}
