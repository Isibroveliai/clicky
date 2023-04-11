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

	void Awake()
    {
		tab = GetComponentInParent<ResearchTabManager>();
		image = GetComponent<Image>();
		image.sprite = node.sprite;
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
		tab.UpdateResearchAdditionalText("", Color.white);
		tab.UpdateResearchDescription("");
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		var researched = IsResearched();
		if (!node.CanBuy() && !researched)
		{
			tab.UpdateResearchAdditionalText("Not enough currency", Color.red);
		}
		else if (researched)
		{
			tab.UpdateResearchAdditionalText("Unlocked!", Color.white);
		}

		tab.UpdateResearchDescription(node.description);
	}
	public void ChangeButtonState(bool state) => button.interactable = state;
	
}
