using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


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
	public List<ResearchNode> next;
	
	public List<Upgrade> unlockUpgrades;
	public bool revealed = false;

	public UnityEvent onBuyEvent;

	public bool CanBuy()
	{
		GameManager mng = GameManager.instance;
		return mng.data.currency >= currencyCost;
	
	}

	public void OnBuy()
	{
		onBuyEvent.Invoke();
	}

	public bool IsResearched()
	{
		return GameManager.instance.data.unlockedResearch.Contains(id);
	}
}
