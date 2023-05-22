using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu]
public class Upgrade : ScriptableObject
{
	public bool instantUnlock;

	[ScriptableObjectId]
	public string id;

	public string displayName;
	[TextArea(1, 3)]
	public string description;
	public HugeNumber baseCurrencyCost;
	public int energyUsage;
	public float energyCapRaise;
	public HugeNumber currencyGeneration;
	public int currencyPerClick;
	public float energyConsumptionDecrease;
	public int researchProduction;
	
	public Texture icon;

	public bool CanBuy()
	{
		GameManager mng = GameManager.instance;
		return mng.data.currency >= baseCurrencyCost;
	}

	public void Buy()
	{
		GameManager manager = GameManager.instance;
		manager.BuyUpgrade(this);
	}
}
