using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResearchNode", menuName = "clicky/ResearchNode", order = 0)]
public class ResearchNode : ScriptableObject
{
    [ScriptableObjectId]
	public string id;
	public string displayName;
	[TextArea(1, 3)]
	public string description;
	public HugeNumber currencyCost;
	public int researchCost;
	public Sprite sprite;
	public bool instantUnlock;
	public List<ResearchNodeButton> next;

	public List<Upgrade> unlockUpgrades;
	
  	public bool CanBuy()
	{
		GameManager mng = GameManager.instance;
		return mng.data.currency >= currencyCost;
	}
}
