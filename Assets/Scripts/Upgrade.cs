using UnityEngine;
using System;
using UnityEditor;

[CreateAssetMenu()]
public class Upgrade : ScriptableObject
{
    [ScriptableObjectId]
    public string id;

    public string name;
    public string description;
    public int baseCost;
    public int generation;

    public bool CanBuy()
    {
        GameManager manager = GameManager.Instance;
        return manager.Score >= baseCost;
    }

    public void Buy()
    {
        GameManager manager = GameManager.Instance;
        manager.Score -= baseCost;
        manager.CurrentGeneration += generation;

        if (!manager.UpgradeCounts.ContainsKey(id))
        {
            manager.UpgradeCounts.Add(id, 0);
        }
        manager.UpgradeCounts[id]++;
    }
}
