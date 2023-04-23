using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

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

	public GameObject gameOverTab;

	public TMP_Text currencyCounter;
	public TMP_Text energyCounter;
	public TMP_Text researchCounter;

	public Color energySafeColor;
	public Color energyDangerColor;

	void Start()
	{
		gameOverTab.SetActive(false);
		activeTab = tabs[0];
		foreach (UITab tab in tabs)
		{		
			tab.button.onClick.AddListener(() => { ChangeTab(tab); });
			tab.panel.SetActive(false);
		}

		GameManager mng = GameManager.instance;
		
		//fixes display issue on start when save date is loaded
		UpdateEnergyDisplayDanger(mng.GetEnergyUsage() >= mng.maxEnergy);
		UpdateEnergyDisplay(mng.GetEnergyUsage(), mng.maxEnergy);
		UpdateScoreDisplay(mng.data.currency);
		UpdateResearchSpeedDisplay(mng.researchProduction);
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
		energyCounter.color = warning ? energyDangerColor : energySafeColor;
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
}
