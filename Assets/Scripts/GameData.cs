using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
	public DateTime lastSave;
	public HugeNumber currency;
	public Dictionary<string, int> upgradeCounts;
	public HashSet<string> disabledUpgrades;
	public List<string> unlockedResearch;
	public string activeResearch;
	public float researchProgress;

	public GameData()
	{
		activeResearch = null;
		researchProgress = 0;
		lastSave = DateTime.MinValue;
		currency = new HugeNumber(0);
		unlockedResearch = new List<string>();
		upgradeCounts = new Dictionary<string, int>();
		disabledUpgrades = new HashSet<string>();
	}
}