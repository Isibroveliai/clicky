using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    TMP_Text currencyCounter;
    Button currencyGenerator;
    Button upgradeButton;
    GameObject upgradeTab;
    void Start()
    {
        currencyCounter = GameObject.Find("/UI/CurrencyCounter").GetComponent<TMP_Text>();
        currencyGenerator = GameObject.Find("UI/CurrencyGenerator/Button").GetComponent<Button>();
        upgradeTab = GameObject.Find("UI/UpgradesTab");
        upgradeButton = GameObject.Find("UI/Menubar/Upgrades").GetComponent<Button>();
        currencyGenerator.onClick.AddListener(ClickerManager.Instance.GenerateCurrency);
        upgradeTab.SetActive(false);
        upgradeButton.onClick.AddListener(ChangeToUpgradeTab);
    }

    // Update is called once per frame
    void Update()
    {
        currencyCounter.text = ClickerManager.Instance.Score.ToString();
        
    }
    void ChangeToUpgradeTab()
    {
        upgradeTab.SetActive(!upgradeTab.activeSelf);

    }
}
