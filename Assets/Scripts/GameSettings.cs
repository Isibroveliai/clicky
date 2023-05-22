using System;

[Serializable]
public class GameSettings
{
	public int windowState;
	public string windowStateName;
	public float volumeLevel;
	public GameSettings()
	{
		windowState = 0;
		windowStateName = "Small";
		volumeLevel = 50f;
	}
}