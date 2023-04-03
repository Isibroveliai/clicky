using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ResearchNode", menuName = "clicky/ResearchNode", order = 0)]
public class ResearchNode : ScriptableObject
{
    [ScriptableObjectId]
	public string id;
	public string displayName;
	[TextArea(1, 3)]
	public string description;
	public int currencyCost;
	public int researchCost;
	public Sprite sprite;
	public bool instantUnlock;


	public List<Upgrade> unlockUpgrades;
	
  	public bool CanBuy()
	{
		GameManager manager = GameManager.instance;
		return manager.currency >= currencyCost;
	}
}
