using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System;

[Serializable]
public class SaveObject
{
	public GameData data;
	public GameSettings settings;
}

public static class SaveManager 
{
	public static string path = Path.Combine(Application.persistentDataPath, "clicky.sav");

	public static void Save(SaveObject save)
	{
		save.data.lastSave = DateTime.Now;

		FileStream fs = new FileStream(path, FileMode.Create);
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(fs, save);
		fs.Close();
	}

	public static SaveObject Load()
	{
		if (!File.Exists(path)) 
		{
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
			// TODO: Display error to player
			Debug.Log("Failed to deserialize");
		}

		return null;
	}

	public static void DeleteSave()
	{
		File.Delete(path);
	}

}
