using System;

[Serializable]
public class GameSettings
{
	public int windowState;
	public string windowStateName;
	public float volumeLevel;
	public GameSettings()
	{
		windowState = 1;
		windowStateName = "Large";
		volumeLevel = 50f;
	}
}