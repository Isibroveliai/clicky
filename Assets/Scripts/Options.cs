using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

//maybe copy all options button click logic here?
public class Options : MonoBehaviour
{
	const string dateFormat = "yyyy-dd-MM HH:mm:ss";
	
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
			UpdateSaveInfoText(mng.data.lastSave.ToString(dateFormat));
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
		SaveManager.Save(mng.GetSaveObject());
		UpdateSaveInfoText(DateTime.Now.ToString(dateFormat));
	}

	public void LoadData()
	{
		mng.LoadSaveFile();
		UpdateSaveInfoText(mng.data.lastSave.ToString(dateFormat));
	}

	public void DeleteSave()
	{
		SaveManager.DeleteSave();
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
		AudioManager.instance.UpdateVolume(volumeSlider.value);
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
