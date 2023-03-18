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
	public uint energyUsage;
	public int generation;

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
