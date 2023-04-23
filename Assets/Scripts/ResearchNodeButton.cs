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
		button = GetComponent<Button>();
		button.onClick.AddListener(() => StartResearch());
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
		if (!node.revealed)
		{
			tab.UpdateResearchDescription("Progress research further to reveal...");
			tab.UpdateResearchAdditionalText("", false, defaultColor);
			return;
		}
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
	public void OnUnlock()
	{
		foreach(ResearchNodeButton neighbor in next)
		{
			neighbor.ChangeSprite();
			neighbor.node.revealed = true;		
		}
		
	}
	public void ChangeSprite() => image.sprite = node.sprite;
	
}
