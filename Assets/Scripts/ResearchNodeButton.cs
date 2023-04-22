using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ResearchNodeButton : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
	public ResearchNode node;
	ResearchTabManager tab;
	public List<ResearchNodeButton> next;

	private Button button;
	private Image image;
	UIManager ui;
	Color defaultColor;
	Color warningColor;
	void Awake()
    {
		ui = GetComponentInParent<UIManager>();
		defaultColor = ui.energySafeColor;
		warningColor = ui.energyDangerColor;
		tab = GetComponentInParent<ResearchTabManager>();
		image = GetComponent<Image>();
		image.sprite = node.sprite;
		button = GetComponent<Button>();
		button.onClick.AddListener(() => StartResearch());
		tab.UpdateResearchAdditionalText("", false, defaultColor);
		tab.UpdateResearchDescription("");
	}
	void StartResearch()
	{
		if (!node.CanBuy()) return;
		button.interactable = false;
		GameManager manager = GameManager.instance;
		manager.StartResearch(node);
		AudioManager.PlayButtonClick();
	}

	public bool IsResearched()
	{
		return GameManager.instance.data.unlockedResearch.Contains(node.id);
	}
	
	public void OnPointerExit(PointerEventData eventData)
	{
		tab.UpdateResearchAdditionalText("", false, defaultColor);
		tab.UpdateResearchDescription("");
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		var researched = IsResearched();
		if ( !researched)
		{
			string text = string.Format("Cost: {0}", node.currencyCost);
			tab.UpdateResearchAdditionalText(text, false, defaultColor);
			if(!node.CanBuy())
			{
				tab.UpdateResearchAdditionalText("\nNot enough currency!", true, warningColor);
			}			
		}
		else if (researched)
		{
			tab.UpdateResearchAdditionalText("Unlocked!", false, defaultColor);
		}

		tab.UpdateResearchDescription(node.description);
	}
	public void ChangeButtonState(bool state) => button.interactable = state;
	
}
