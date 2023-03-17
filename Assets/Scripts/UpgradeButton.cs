using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// TODO: Make info about upgrade be visible in editor.
public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public Upgrade upgrade;

	private TMP_Text label;
	private TMP_Text cCost;
	//private TMP_Text eCost;
	private Button button;

	void Start()
	{
		GameManager manager = GameManager.instance;
		label = transform.Find("Label").GetComponent<TMP_Text>();
		cCost = transform.Find("CurrencyCost").GetComponent<TMP_Text>();
		//eCost = transform.Find("EnergyCost").GetComponent<TMP_Text>();
		button = GetComponent<Button>();

		label.text = upgrade.displayName;
		cCost.text = upgrade.baseCost.ToString();
		//eCost.text = upgrade.energyCost.ToString();
		button.onClick.AddListener(() => upgrade.Buy());
		
	}
	
	void FixedUpdate()
	{
		// TODO: Refactor, to not check can buy every frame.
		// Add "OnClick" event to ClickerManager to solve this problem.
		button.interactable = upgrade.CanBuy();
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
