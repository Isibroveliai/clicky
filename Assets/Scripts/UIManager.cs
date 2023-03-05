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
    Button currencyGenerator;
    Button upgradeButton;
    Button optionsButton;
    GameObject upgradeTab;
    GameObject optionsTab;

    [SerializeField]
    GameObject currentlyActiveTab;
    void Start()
    {
        manager = GameManager.Instance;

        currencyCounter = GameObject.Find("/UI/Counters/CurrencyCounter").GetComponent<TMP_Text>();
        currencyGenerator = GameObject.Find("UI/CurrencyGenerator/Button").GetComponent<Button>();
        upgradeTab = GameObject.Find("UI/UpgradesTab");
        upgradeButton = GameObject.Find("UI/Menubar/Upgrades").GetComponent<Button>();
        optionsTab = GameObject.Find("UI/OptionsTab");
        optionsButton = GameObject.Find("UI/Menubar/Options").GetComponent<Button>();

        //currencyGenerator.onClick.AddListener(ClickerManager.Instance.GenerateCurrency);
        //upgradeButton.onClick.AddListener(delegate { ChangeTab(upgradeTab); });
        //optionsButton.onClick.AddListener(delegate { ChangeTab(optionsTab); });
        optionsButton.onClick.AddListener(() => { ChangeTab(optionsTab); });
        upgradeButton.onClick.AddListener(() => { ChangeTab(upgradeTab); });


        currentlyActiveTab = optionsTab; // just so it isnt null
        upgradeTab.SetActive(false);
        optionsTab.SetActive(false); 
    }

    void Update()
    {
        currencyCounter.text = ((ulong)manager.Score).ToString();
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
