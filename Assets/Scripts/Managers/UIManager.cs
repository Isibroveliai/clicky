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
	public GameObject eventTab;
	public TMP_Text eventText;

	public GameObject gameOverTab;

	public TMP_Text currencyCounter;

	public TMP_Text upgradeDescriptionText;
	public TMP_Text researchDescriptionText;
	public TMP_Text researchCantBuyText;

	public GameObject upgradeButtonContainer;
	public GameObject upgradeButtonPrefab;

	private UITab activeTab;

	private Color safeColor;
	private Color dangerColor;
	void Start()
	{
		eventTab.SetActive(false);
		gameOverTab.SetActive(false);

		activeTab = tabs[0];
		for (int i = 0; i < tabs.Length; i++)
		{
			UITab tab = tabs[i];
			tab.panel.SetActive(false);
			tab.button.onClick.AddListener(() => { ChangeTab(tab); });
		}

		safeColor = new Color(0.7686275f, 0.6078432f, 0.3372549f);
		dangerColor = new Color(0.2509804f, 0.1372549f, 0.1176471f, 1);
	}

	public void UpdateUpgradeDescription(string text)
	{
		upgradeDescriptionText.text = text;
	}
	public void UpdateResearchDescription(string text)
	{
		researchDescriptionText.text = text;
	}
	public void UpdateResearchCantBuyText(string text)
	{
		researchCantBuyText.text = text;
	}

	public void SetGameOverShown(bool isShown)
	{
		gameOverTab.SetActive(isShown);
	}

	public void UpdateScoreDisplay(ulong score)
	{
		currencyCounter.text = score.ToString();
	}

	public void UpdateEnergyDisplay(float current, float max)
	{
		energyCounter.text = string.Format("{0:0} kW/{1:0} kW", current, max);
	}
	public void UpdateEnergyDisplayDanger(bool warning)
	{
		if (warning) energyCounter.color = dangerColor;
		else energyCounter.color = safeColor;
	}	

	public void SetEventTextShown(bool isShown)
	{
		eventTab.SetActive(isShown);
	}
	public void SetEventText(string text)
	{
		eventText.text = text;
	}

	// TODO: Add remove? idk if we will need it
	public void AppendUpgradeButton(Upgrade upgrade)
	{
		var button = Instantiate(upgradeButtonPrefab);
		var upgradeComponent = button.GetComponent<UpgradeButton>();
		upgradeComponent.upgrade = upgrade;
		button.transform.SetParent(upgradeButtonContainer.transform, false);
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
