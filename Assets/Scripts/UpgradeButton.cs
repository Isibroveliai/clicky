using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

// TODO: Make info about upgrade be visible in editor.
public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public Upgrade upgrade;

	private TMP_Text nameLabel;
	private TMP_Text currencyCostLabel;
	private TMP_Text energyCostLabel;
	private TMP_Text boughtCountLabel;

	private Button button;

	void Start()
	{
		GameManager manager = GameManager.instance;
		nameLabel = transform.Find("Button/Label").GetComponent<TMP_Text>();
		currencyCostLabel = transform.Find("Button/CurrencyCost").GetComponent<TMP_Text>();
		energyCostLabel = transform.Find("Button/EnergyCost").GetComponent<TMP_Text>();
		boughtCountLabel = transform.Find("Counter").GetComponent<TMP_Text>();
		button = transform.Find("Button").GetComponent<Button>();
		RawImage icon = transform.Find("Icon").GetComponent<RawImage>();

		UpdateCountLabel();
		nameLabel.text = upgrade.displayName;
		icon.texture = upgrade.icon;
		currencyCostLabel.text = string.Format("{0}$", upgrade.baseCurrencyCost);
		energyCostLabel.text = string.Format("{0}e", upgrade.energyUsage);
		button.onClick.AddListener(() => upgrade.Buy());

		GameManager.OnUpgradeBought += (boughtUpgrade) => {
			if (boughtUpgrade == upgrade)
				UpdateCountLabel();
		};
	}
	
	void FixedUpdate()
	{
		// TODO: Refactor, to not check can buy every frame.
		// Add "OnClick" event to ClickerManager to solve this problem.
		button.interactable = upgrade.CanBuy();
	}

	public void UpdateCountLabel()
	{
		GameManager manager = GameManager.instance;
		boughtCountLabel.text = string.Format("x {0}", manager.upgradeCounts.GetValueOrDefault(upgrade.id, 0));
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		UIManager ui = GameObject.Find("/UI").GetComponent<UIManager>();
		ui.UpdateUpgradeDescription("");
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		UIManager ui = GameObject.Find("/UI").GetComponent<UIManager>();
		ui.UpdateUpgradeDescription(upgrade.description);
	}
}
