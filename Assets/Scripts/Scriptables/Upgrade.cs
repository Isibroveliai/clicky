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
    public int baseCost;
    public int generation;

    public bool CanBuy()
    {
        GameManager manager = GameManager.instance;
        return manager.score >= baseCost;
    }

    public void Buy()
    {
        GameManager manager = GameManager.instance;
        manager.score -= baseCost;
        manager.currentGeneration += generation;

        if (!manager.upgradeCounts.ContainsKey(id))
        {
            manager.upgradeCounts.Add(id, 0);
        }
        manager.upgradeCounts[id]++;
    }
}
