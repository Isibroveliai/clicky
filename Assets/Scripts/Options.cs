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
	GameManager mng;
	[SerializeField]
	TMP_Text volumeValueText;
	[SerializeField]
	TMP_Text saveInfoText;
	[SerializeField]
	Slider volumeSlider;
	[SerializeField]
	GameObject changeWindowButtonObj;

	void Start()
	{
	
		mng = GameManager.instance;
		
		if(!mng.data.lastSave.Equals(DateTime.MinValue))
		{
			UpdateSaveInfoText(mng.data.lastSave.ToString("yyyy-dd-MM HH:mm:ss"));
		}
		else UpdateSaveInfoText("");

		UpdateWindowChangeButtonText(mng.settings.windowStateName);
		UpdateVolumeText(mng.settings.volumeLevel.ToString());
		SetVolumeValue(mng.settings.volumeLevel);
		

		
	}
	public void IncrementWindowState()
	{
		mng.settings.windowState++;
		if (mng.settings.windowState >= 2)
		{
			mng.settings.windowState = 0;
		}
		ChangeWindowSize(mng.settings.windowState);
	}
	public void ChangeWindowSize(int state)
	{
		switch (state)
		{
			case 0:
				mng.settings.windowStateName = "Small";
				Screen.SetResolution(640, 480, false);
				break;

			case 1:
				mng.settings.windowStateName = "Large";
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
		UpdateWindowChangeButtonText(mng.settings.windowStateName);
	}
	public void QuitGame()
	{
		SaveGame();
		Application.Quit();
	}

	public void SaveGame()
	{
		SaveManager.Save(mng.GetData(), mng.savePath);
		UpdateSaveInfoText(DateTime.Now.ToString("yyyy-dd-MM HH:mm:ss"));
	}

	public void LoadData()
	{
		mng.LoadData();
		UpdateSaveInfoText(mng.data.lastSave.ToString("yyyy-MM-dd HH:mm:ss"));
	}
	public void DeleteSave()
	{
		if(File.Exists(mng.savePath))
		{
			File.Delete(mng.savePath);
			Debug.Log("Save deleted");
			return;
		}
		Debug.Log("Save file not found");
	}

	public void SetVolumeValue(float value)
	{
		volumeSlider.value = value;
		UpdateVolumeText(value.ToString());
	}
	public void UpdateVolumeText(string text)
	{
		volumeValueText.text = text;
	}
	public void OnVolumeChange()
	{
		UpdateVolumeText(volumeSlider.value.ToString());
		mng.settings.volumeLevel = volumeSlider.value;
	}
	public void UpdateWindowChangeButtonText(string text)
	{
		changeWindowButtonObj.GetComponentInChildren<TMP_Text>().text = text;
	}
	public void UpdateSaveInfoText(string text)
	{
		saveInfoText.text = text;
	}

	
}
