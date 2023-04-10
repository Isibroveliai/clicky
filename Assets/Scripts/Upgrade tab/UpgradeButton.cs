using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

// TODO: Make info about upgrade be visible in editor.
public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public UpgradeTab upgradeTab;
	public Upgrade upgrade;

	private AudioClip[] buyUpgradeSound;
	private TMP_Text nameLabel;
	private TMP_Text currencyCostLabel;
	private TMP_Text energyCostLabel;
	private TMP_Text boughtCountLabel;
	private AudioSource audioPlayer;

	public Color normalTextColor;
	public Color disabledTextColor;
	public Color currencyHighlightColor;

	private Button button;
	private bool isButtonHovering = false;
	private bool isButtonPressed = false;
	private bool isDisabled = false;

	private float lastBuy = 0;
	private int buyUpgradeSoundIdx = 0;
	private float buySpeed = 0.8f;

	void Start()
	{
		buyUpgradeSound = AudioManager.instance.buyUpgrade;

		nameLabel = transform.Find("Button/Label").GetComponent<TMP_Text>();
		currencyCostLabel = transform.Find("Button/CurrencyCost").GetComponent<TMP_Text>();
		energyCostLabel = transform.Find("Button/EnergyCost").GetComponent<TMP_Text>();
		boughtCountLabel = transform.Find("Counter").GetComponent<TMP_Text>();
		button = transform.Find("Button").GetComponent<Button>();
		audioPlayer = GetComponent<AudioSource>();
		RawImage icon = transform.Find("Icon frame/Icon mask/Icon").GetComponent<RawImage>();

		nameLabel.text = upgrade.displayName;
		icon.texture = upgrade.icon;
		currencyCostLabel.text = string.Format("{0}$", upgrade.baseCurrencyCost);
		energyCostLabel.text = string.Format("{0} kW", upgrade.energyUsage);
	}
	
	void Update()
	{
		UpdateCountLabel();
	}

	void FixedUpdate()
	{
		// TODO: Refactor, to not check can buy every frame.
		// Add "OnClick" event to ClickerManager to solve this problem.
		if (isDisabled)
		{
			button.interactable = false;
		}
		else
		{
			button.interactable = upgrade.CanBuy();


			Color buttonTextColor = button.interactable ? normalTextColor : disabledTextColor;
			currencyCostLabel.color = buttonTextColor;
			energyCostLabel.color = buttonTextColor;
			nameLabel.color = buttonTextColor;

			if (!button.interactable && isButtonHovering)
			{
				currencyCostLabel.color = currencyHighlightColor;
			}

			float now = Time.time;
			float buyInterval = buyUpgradeSound[buyUpgradeSoundIdx].length / buySpeed;
			if ((now - lastBuy) > buyInterval)
			{
				if (isButtonPressed && upgrade.CanBuy())
				{
					BuyUpgrade();
					buySpeed = Math.Min(buySpeed * 1.1f, 4f);
				}
				else
				{
					buySpeed = 1;
				}
			}
		}
	}

	public void BuyUpgrade()
	{
		upgrade.Buy();
		lastBuy = Time.time;
		buyUpgradeSoundIdx = (buyUpgradeSoundIdx + 1) % buyUpgradeSound.Length;
		audioPlayer.pitch = buySpeed;
		audioPlayer.Stop();
		audioPlayer.clip = buyUpgradeSound[buyUpgradeSoundIdx];
		audioPlayer.Play();
	}

	public void UpdateCountLabel()
	{
		GameManager mng = GameManager.instance;
		boughtCountLabel.text = string.Format("x {0}", mng.data.upgradeCounts.GetValueOrDefault(upgrade.id, 0));
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		upgradeTab.descriptionText.text = "";
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		upgradeTab.descriptionText.text = upgrade.description;
	}

	public void OnButtonEnter()
	{
		isButtonHovering = true;
	}

	public void OnButtonExit()
	{
		isButtonHovering = false;
	}

	public void OnButtonPressed()
	{
		if (!button.IsInteractable()) return;

		isButtonPressed = true;
	}

	public void OnButtonReleased()
	{
		isButtonPressed = false;
	}

	public void DisableButtonPressed()
	{
		isDisabled = !isDisabled;
		if (isDisabled)
		{
			GameManager.instance.DisableUpgrade(upgrade);
		}
		else
		{
			GameManager.instance.EnableUpgrade(upgrade);
		}
	}
}
