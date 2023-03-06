using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Numerics;
using System;

public class UIManager : MonoBehaviour
{
    GameManager manager;

    [SerializeField]
    TMP_Text currencyCounter;
    [SerializeField]
    Button currencyGeneratorButton;
    [SerializeField]
    Button upgradeButton;
    [SerializeField]
    Button optionsButton;
    [SerializeField]
    GameObject upgradeTab;
    [SerializeField]
    GameObject optionsTab;
    [SerializeField]
    GameObject gameOverTab;
    [SerializeField]
    Button restartButton;

    GameObject currentlyActiveTab;

    void Start()
    {
        manager = GameManager.Instance;

        currencyGeneratorButton.onClick.AddListener(manager.GenerateCurrency);
        //upgradeButton.onClick.AddListener(delegate { ChangeTab(upgradeTab); });
        //optionsButton.onClick.AddListener(delegate { ChangeTab(optionsTab); });
        optionsButton.onClick.AddListener(() => { ChangeTab(optionsTab); });
        upgradeButton.onClick.AddListener(() => { ChangeTab(upgradeTab); });
        restartButton.onClick.AddListener(() => { manager.RestartScene(); });


        currentlyActiveTab = optionsTab; // just so it isnt null
        upgradeTab.SetActive(false);
        optionsTab.SetActive(false);
        gameOverTab.SetActive(false);
    }

    void Update()
    {
        currencyCounter.text = ((ulong)manager.Score).ToString();
        if(manager.currentEnergy <= 0)
        {
            gameOverTab.SetActive(true);
        }
    }

    public void ChangeTab(GameObject obj)
    {
        obj.SetActive(!obj.activeSelf);
        if (currentlyActiveTab != obj)
        {
            currentlyActiveTab.SetActive(false);
            currentlyActiveTab = obj;
        }
        
    }
}
