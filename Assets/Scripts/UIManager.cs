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
    TMP_Text currencyCounter;
    TMP_Text energyCounter;
    Button currencyGenerator;
    Button upgradeButton;
    Button optionsButton;
    GameObject upgradeTab;
    GameObject optionsTab;
    GameObject gameOverTab;
    Button restartButton;

    [SerializeField]
    GameObject currentlyActiveTab;
    void Start()
    {
        manager = GameManager.Instance;

        currencyCounter = GameObject.Find("/UI/Counters/CurrencyCounter").GetComponent<TMP_Text>();
        energyCounter = GameObject.Find("/UI/Counters/EnergyCounter").GetComponent<TMP_Text>();
        currencyGenerator = GameObject.Find("UI/CurrencyGenerator/Button").GetComponent<Button>();
        upgradeTab = GameObject.Find("UI/UpgradesTab");
        upgradeButton = GameObject.Find("UI/Menubar/Upgrades").GetComponent<Button>();
        optionsTab = GameObject.Find("UI/OptionsTab");
        optionsButton = GameObject.Find("UI/Menubar/Options").GetComponent<Button>();
        gameOverTab = GameObject.Find("UI/GameOverTab");
        restartButton = GameObject.Find("UI/GameOverTab/Button").GetComponent<Button>();

        currencyGenerator.onClick.AddListener(manager.GenerateCurrency);
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
        energyCounter.text = ((int)manager.currentEnergy).ToString() + "%";

        if (manager.currentEnergy <= 0)
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
