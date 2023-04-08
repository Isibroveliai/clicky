using UnityEngine;

[CreateAssetMenu]
public class Upgrade : ScriptableObject
{
	[ScriptableObjectId]
	public string id;

	public string displayName;
	[TextArea(1, 3)]
	public string description;
	public HugeNumber baseCurrencyCost;
	public int energyUsage;
	public float energyCapRaise;
	public HugeNumber currencyGeneration;
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
