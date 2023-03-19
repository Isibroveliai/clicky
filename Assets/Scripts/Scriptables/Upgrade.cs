using UnityEngine;
using System;
using UnityEditor;

[CreateAssetMenu]
public class Upgrade : ScriptableObject
{
	[ScriptableObjectId]
	public string id;

	public string displayName;
	[TextArea(1, 3)]
	public string description;
	public int baseCurrencyCost;
	public int energyUsage;
	public int currencyGeneration;
	public float energyConsumptionDecrease;
	public int researchProduction;

	public Texture icon;

	public bool CanBuy()
	{
		GameManager manager = GameManager.instance;
		return manager.currency >= baseCurrencyCost;
	}

	public void Buy()
	{
		GameManager manager = GameManager.instance;
		manager.BuyUpgrade(this);
	}
}
