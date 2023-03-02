using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    public float Score = 0;

    public Dictionary<string, int> UpgradeCounts;

    public float CurrentGeneration = 0;

    public GameManager()
    {
        UpgradeCounts = new Dictionary<string, int>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        Score += CurrentGeneration * Time.deltaTime;
    }

    public static void GenerateCurrency() => Instance.Score++;

}
