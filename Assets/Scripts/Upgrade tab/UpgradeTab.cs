using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeTab : MonoBehaviour
{
	public GameObject buttonContainer;
	public GameObject buttonPrefab;
	public TMP_Text descriptionText;

	public void Start()
	{
		GameManager manager = GameManager.instance;
		foreach (var upgrade in manager.unlockedUpgrades)
		{
			AppendUpgradeButton(upgrade);
		}

		GameManager.OnUpgradeUnlocked += (upgrade) =>
		{
			AppendUpgradeButton(upgrade);
		};
	}

	public void UpdateUpgradeDescription(string text)
	{
		descriptionText.text = text;
	}

	public void AppendUpgradeButton(Upgrade upgrade)
	{
		var button = Instantiate(buttonPrefab);
		var upgradeComponent = button.GetComponent<UpgradeButton>();
		upgradeComponent.upgrade = upgrade;
		upgradeComponent.upgradeTab = this;
		button.transform.SetParent(buttonContainer.transform, false);
	}
}
