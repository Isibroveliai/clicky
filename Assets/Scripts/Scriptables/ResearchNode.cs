using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ResearchNode", menuName = "clicky/ResearchNode", order = 0)]
public class ResearchNode : ScriptableObject
{
    [ScriptableObjectId]
	string id;
	public string displayName;
	public string description;
	public int baseCost;
	public Sprite sprite;

	public Upgrade upgrade;
	
  	public bool CanBuy()
	{
		GameManager manager = GameManager.instance;
		return manager.score >= baseCost;
	}
	
	public void Buy()
	{
		GameManager manager = GameManager.instance;
		manager.score -= baseCost;
		
		if (!manager.researchUnlocks.Contains(upgrade.id))
		{
			manager.researchUnlocks.Add(id);
		}
		
	}
	
}
