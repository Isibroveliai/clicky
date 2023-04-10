using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

// TODO: Make this a singleton, so it is acccesible everywhere
public class UIManager : MonoBehaviour
{
	[Serializable]
	public class UITab
	{
		public GameObject panel;
		public Button button;
	}

	public UITab[] tabs;
	private UITab activeTab;

	public GameObject eventTab;
	public TMP_Text eventText;

	public GameObject gameOverTab;

	public TMP_Text currencyCounter;

	public UpgradeTab upgradeTab;

	[Header("Options tab")]
	public GameObject changeWindowButtonObj;
	public TMP_Text saveInfoText;
	public Slider volumeSlider;
	public TMP_Text volumeValueText;

	[Header("Energy")]
	public TMP_Text energyCounter;
	public Color energySafeColor;
	public Color energyDangerColor;

	[Header("Research")]
	public ResearchTabManager researchTabManager;
	public TMP_Text researchCounter;
	public TMP_Text researchDescriptionText;
	public TMP_Text researchAdditionalText;
	public TMP_Text currentResearchLabel;
	public RectTransform researchProgressbar;
	public Color startingLineColor;
	public Color finishedLineColor;
	public Color inProgressLineColor;
	private float initialProgressbarSize;

	void Start()
	{
		eventTab.SetActive(false);
		gameOverTab.SetActive(false);
		activeTab = tabs[0];
		for (int i = 0; i < tabs.Length; i++)
		{
			UITab tab = tabs[i];
			//tab.panel.SetActive(true); //to do all their Start() methods
			
			tab.button.onClick.AddListener(() => { ChangeTab(tab); });
			//if (tab.panel.name == "ResearchTab") researchTabManager = tab.panel.GetComponent<ResearchTabManager>(); // TODO: find another way to do this
			//else if(tab.panel.name == "Options") 
			//tab.panel.SetActive(false);
		}

		initialProgressbarSize = researchProgressbar.sizeDelta.x;

		UpdateCurrentResearchLabel("");
		UpdateResearchProgress(0);
		UpdateResearchDescription("");
		UpdateResearchAdditionalText("", startingLineColor);

		GameManager mng = GameManager.instance;
		
		//fixes display issue on start when save date is loaded
		UpdateEnergyDisplayDanger(mng.GetEnergyUsage() >= mng.maxEnergy);
		UpdateEnergyDisplay(mng.GetEnergyUsage(), mng.maxEnergy);
		UpdateScoreDisplay(mng.data.currency);
		UpdateResearchSpeedDisplay(mng.researchProduction);
	}

	public void UpdateResearchDescription(string text)
	{
		researchDescriptionText.text = text;
	}
	public void UpdateResearchAdditionalText(string text, Color color)
	{
		researchAdditionalText.text = text;
		researchAdditionalText.color = color;
	}

	public void SetGameOverShown(bool isShown)
	{
		gameOverTab.SetActive(isShown);
	}

	public void UpdateScoreDisplay(HugeNumber score)
	{
		currencyCounter.text = string.Format("{0} $", score);
	}

	public void UpdateEnergyDisplay(float current, float max)
	{
		energyCounter.text = string.Format("{0:0}/{1:0} kW", current, max);
	}
	public void UpdateEnergyDisplayDanger(bool warning)
	{
		if (warning) energyCounter.color = energyDangerColor;
		else energyCounter.color = energySafeColor;
	}	

	public void SetEventTextShown(bool isShown)
	{
		eventTab.SetActive(isShown);
	}
	public void SetEventText(string text)
	{
		eventText.text = text;
	}

	public void UpdateResearchSpeedDisplay(float researchSpeed)
	{
		researchCounter.text = researchSpeed.ToString();
	}

	public void ChangeTab(UITab tab)
	{
		tab.panel.SetActive(!tab.panel.activeSelf);
		if (activeTab != tab)
		{
			activeTab.panel.SetActive(false);
			activeTab = tab;
		}
	}

	public void UpdateResearchProgress(float percent)
	{
		float width = initialProgressbarSize * percent;
		float height = researchProgressbar.sizeDelta.y;
		researchProgressbar.sizeDelta = new Vector2(width, height);
	}

	public void UpdateCurrentResearchLabel(string researchName)
	{
		if (researchName == "")
		{
			currentResearchLabel.text = "";
		} else
		{
			currentResearchLabel.text = $"Researching '{researchName}'...";
		}
	}
}
