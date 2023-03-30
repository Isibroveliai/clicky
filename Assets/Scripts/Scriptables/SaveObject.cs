using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveObject
{
	public float currency;

	public float researchProduction;

	public float currencyGeneration;

	public float energyUsage;

	public float maxEnergy;

	public float clickMultiplier;

	public Dictionary<string, int> upgradeCounts;
	public List<string> unlockedUpgrades;
	public List<string> unlockedResearch;
	

}
