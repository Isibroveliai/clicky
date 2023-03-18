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
		if (manager.unlockedResearch.Contains(this)) return;

		manager.score -= baseCost;
		manager.unlockedResearch.Add(this);

		if (upgrade)
		{
			manager.UnlockUpgrade(upgrade);
		}
	}
}
