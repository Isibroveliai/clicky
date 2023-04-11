using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ResearchNodeButton : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
	public ResearchNode node;
	public List<ResearchNodeButton> next;

	private Button button;
	private Image image;
	UIManager ui;

	void Awake()
    {
		ui = GameObject.Find("/UI").GetComponent<UIManager>();
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
		ui.UpdateResearchAdditionalText("", ui.energySafeColor);
		ui.UpdateResearchDescription("");
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		var researched = IsResearched();
		if (!node.CanBuy() && !researched)
		{
			ui.UpdateResearchAdditionalText("Not enough currency", ui.energyDangerColor);
		}
		else if (researched)
		{
			ui.UpdateResearchAdditionalText("Unlocked!", ui.energySafeColor);
		}

		ui.UpdateResearchDescription(node.description);
	}
	public void ChangeButtonState(bool state) => button.interactable = state;
	
}
