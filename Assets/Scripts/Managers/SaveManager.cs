using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System;

[System.Serializable]
public class SaveObject
{
	public DateTime saveTime;
	public float currency;

	public float researchProduction;

	public float currencyGeneration;

	public float energyUsage;

	public float maxEnergy;

	public float clickMultiplier;

	public Dictionary<string, int> upgradeCounts;
	public List<string> unlockedUpgrades;
	public List<string> unlockedResearch;
	public GameSettings settings;
}
public static class SaveManager 
{
	public static void Save(SaveObject save, string path)
	{
		FileStream fs = new FileStream(path, FileMode.Create);
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(fs, save);
		fs.Close();
		Debug.Log("Saved data");
		Debug.Log(path);
		
	}
	public static SaveObject Load(string path)
	{
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

}
