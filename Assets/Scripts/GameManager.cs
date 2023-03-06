using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    public float Score = 0;

    public Dictionary<string, int> UpgradeCounts;

    public float CurrentGeneration = 0;

    public float scoreReductionRate = 1f; //rate of which score reduces

    public float currentEnergy = 100; //if reaches 0, game lost

    public float maxEnergy = 100; //upgradable
    
    public float energyRegenerationRate = 0.001f;

    public bool scoreThresholdReached = false; //100 for prototype?

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
        if (Score > 100)  //number changeable     
            scoreThresholdReached = true;

        Score += CurrentGeneration * Time.deltaTime;
        if (scoreThresholdReached)
        {
            currentEnergy -= scoreReductionRate * Time.deltaTime;
        }
    }

    public void GenerateCurrency()
    {
        Score++;
        currentEnergy = Mathf.Min(maxEnergy, currentEnergy+energyRegenerationRate);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
