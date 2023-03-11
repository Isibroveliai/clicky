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

	public TMP_Text energyCounter;
	public GameObject eventText;

	public GameObject gameOverTab;

	public TMP_Text currencyCounter;

	public TMP_Text upgradeDescriptionText;

	private UITab activeTab;

	void Start()
	{
		eventText.SetActive(false);
		gameOverTab.SetActive(false);

		activeTab = tabs[0];
		for (int i = 0; i < tabs.Length; i++)
		{
			UITab tab = tabs[i];
			tab.panel.SetActive(false);
			tab.button.onClick.AddListener(() => { ChangeTab(tab); });
		}
	}

	public void UpdateUpgradeDescription(string text)
	{
		upgradeDescriptionText.text = text;
	}

	public void SetGameOverShown(bool isShown)
	{
		gameOverTab.SetActive(isShown);
	}

	public void UpdateScoreDisplay(ulong score)
	{
		currencyCounter.text = score.ToString();
	}

	public void UpdateEnergyDisplay(float percent)
	{
		energyCounter.text = string.Format("{0}%", Math.Round(percent * 100));
	}

	public void SetEventTextShown(bool isShown)
	{
		eventText.SetActive(isShown);
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
