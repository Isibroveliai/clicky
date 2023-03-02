using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    
    TMP_Text currencyCounter;
    Button currencyGenerator;
    public  Button upgradeButton;
    public Button optionsButton;
    GameObject upgradeTab;
    GameObject optionsTab;
    void Start()
    {


        currencyCounter = GameObject.Find("/UI/CurrencyCounter").GetComponent<TMP_Text>();
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


        upgradeTab.SetActive(false);
        optionsTab.SetActive(false); 
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
    public void ChangeTab(GameObject obj)
    {
        obj.SetActive(!obj.activeSelf);
    }
}
