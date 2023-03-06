using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.FlowStateWidget;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private static UIManager ui;

    // TODO: Make this read-only through editor
    public float Score = 0;

    public Dictionary<string, int> UpgradeCounts;

    public float CurrentGeneration = 0;

    public float scoreReductionRate = 1f; //rate of which score reduces

    // TODO: Make this read-only through editor
    public float currentEnergy = 100; //if reaches 0, game lost

    public float maxEnergy = 100; //upgradable
    
    public float energyRegenerationRate = 0.001f;

    public bool scoreThresholdReached = false; //100 for prototype?
    [SerializeField]
    public float EventCheckTime = 120;

    public float EventTime = 10;

    public bool eventFlag = false;

    public float timer = 0;

    public float CurrentScore;

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
        Setup();
    }

    void Setup()
    {
        ui = GameObject.Find("/UI").GetComponent<UIManager>();
        ui.UpdateUpgradeDescription("");
    }

    void Update()
    {
        var random = new System.Random();

        if (timer > EventCheckTime && !eventFlag)
        {
            timer = 0;
            if (random.Next(10) > 1)
            {
                eventFlag = true;
                CurrentScore = Score;
            }
        }
        else if (eventFlag)
        {
            
            if (timer > EventTime)
            {
                eventFlag = false;
                Debug.Log(string.Format("{0}, {1}", Score, CurrentScore));
                if (CurrentScore + 20 <= Score)
                {
                    Score += 50;
                }
                else
                {
                    Score -= 50;
                }
                timer = 0;
            }
        }
        timer += Time.deltaTime;
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
