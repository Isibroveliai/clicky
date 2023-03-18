using UnityEngine;
using System;
using UnityEditor;

[CreateAssetMenu]
public class Upgrade : ScriptableObject
{
	[ScriptableObjectId]
	public string id;

	public string displayName;
	public string description;
	public int baseCurrencyCost;
	public int energyUsage;
	public int generation;
	public float energyConsumptionDecrease;

	public Texture icon;

	public bool CanBuy()
	{
		GameManager manager = GameManager.instance;
		return manager.score >= baseCurrencyCost;
	}

	public void Buy()
	{
		GameManager manager = GameManager.instance;
		manager.BuyUpgrade(this);
	}
}
