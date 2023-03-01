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
    void Start()
    {
        currencyCounter = GameObject.Find("/UI/CurrencyCounter").GetComponent<TMP_Text>();
        currencyGenerator = GameObject.Find("UI/CurrencyGenerator/Button").GetComponent<Button>();
        currencyGenerator.onClick.AddListener(ClickerManager.Instance.GenerateCurrency);

    }

    // Update is called once per frame
    void Update()
    {
        currencyCounter.text = ClickerManager.Instance.Score.ToString();
        
    }
}
