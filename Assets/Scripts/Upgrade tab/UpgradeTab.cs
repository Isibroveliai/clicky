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
			
			if(!upgrade.instantUnlock)
				AppendUpgradeButton(upgrade);
		}

		GameManager.OnUpgradeUnlocked += (upgrade) =>
		{
			print("ON UPGRADE UNLOCKED");
			if(!upgrade.instantUnlock)
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
		//upgrade.button = button;
		button.transform.SetParent(buttonContainer.transform, false);
	}
	public void DisableButton(Upgrade upgrade)
	{
		Transform buttonObj = gameObject.transform.Find("Upgrades/Viewport/Layout");
		
		for(int i = 0; i < buttonObj.transform.childCount; i ++)
		{
			UpgradeButton child = buttonObj.transform.GetChild(i).gameObject.GetComponent<UpgradeButton>();
			if(child.upgrade.id == upgrade.id)
			{
				child.DisableButtonPressed();
			}
		}
	}
}
