using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.IO;
//maybe copy all options button click logic here?
public class Options : MonoBehaviour
{
	GameManager gm;
	UIManager ui;
	void Start()
	{
		gm = GameManager.instance;
		ui = GameObject.Find("/UI").GetComponent<UIManager>();
	}
	public void IncrementWindowState()
	{
		gm.settings.windowState++;
		if (gm.settings.windowState >= 2)
		{
			gm.settings.windowState = 0;
		}
		ChangeWindowSize(gm.settings.windowState);
	}
	public void ChangeWindowSize(int state)
	{
		switch (state)
		{
			case 0:
				gm.settings.windowStateName = "Small";
				Screen.SetResolution(640, 480, false);
				break;

			case 1:
				gm.settings.windowStateName = "Large";
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
		ui.UpdateWindowChangeButtonText(gm.settings.windowStateName);
	}
	public void QuitGame()
	{
		SaveGame(gm.savePath);
		Application.Quit();
	}
	public void SaveGame(string savePath)
	{
		SaveManager.Save(gm.GetData(), savePath);
		ui.UpdateSaveInfoText(DateTime.Now.ToString("yyyy-dd-MM HH:mm:ss"));
	}

	public void LoadData(string savePath)
	{
		SaveObject save = SaveManager.Load(savePath);
		if (save == null) return;
		gm.SetData(save);
		ui.UpdateSaveInfoText(gm.lastSave.ToString("yyyy-dd-MM HH:mm:ss"));
	}
	public void DeleteSave(string savePath)
	{
		if(File.Exists(savePath))
		{
			File.Delete(savePath);
			Debug.Log("Save deleted");
			return;
		}
		Debug.Log("Save file not found");
	}
}
