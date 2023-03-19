using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ResearchNodeButton : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
	[SerializeField]
	GameObject lineObject;
	[SerializeField]
	ResearchNode node;
	public List<ResearchNodeButton> next;
	public List<ResearchNodeButton> preceding;
	public bool researched = false;

	private Button button;
	private Image image;
	UIManager ui;

	void Start()
    {
		ui = GameObject.Find("/UI").GetComponent<UIManager>();

		image = GetComponent<Image>();
		image.sprite = node.sprite;

		button = GetComponent<Button>();
		button.onClick.AddListener(() => StartResearch());

		foreach (var node in next)
		{
			node.GetComponent<Button>().interactable = false;
			node.preceding.Add(this);
			var line = Instantiate(lineObject, transform);
			LineDrawer drawer = line.GetComponent<LineDrawer>();
			drawer.StartPos = GetComponent<RectTransform>().position;
			drawer.EndPos = node.GetComponent<RectTransform>().position;
			drawer.UpdateColor(ui.startingLineColor);
		}

		GameManager.OnResearchStarted  += OnResearchStarted;
		GameManager.OnResearchFinished += OnResearchFinished;
		GameManager.OnResearchStopped  += OnResearchStopped;
	}

	void StartResearch()
	{
		if (!node.CanBuy()) return;

		button.interactable = false;

		/*
		unlocked = true;
		foreach(var nextNode in next)
		{
			if(!nextNode.unlocked)
			{
				nextNode.GetComponent<Button>().interactable = true;
			}
			UpdateLines(nextNode);
		}
		UpdateLines(this);
		*/

		GameManager manager = GameManager.instance;
		manager.StartResearch(node);
	}

	/*
	void UpdateLines(ResearchNodeButton node)
	{
		foreach (var obj in node.preceding)
		{
			if (!obj.unlocked) continue;
			for (int i = 0; i < obj.next.Count; i++)
			{
				var child = obj.next[i];
				if (child.unlocked)
				{
					obj.transform.GetChild(i).GetComponent<LineDrawer>().SetUnlockedColor();
				}
			}
		}
	}
	*/

	public void OnResearchStarted(ResearchNode research)
	{
		// TODO
	}

	public void OnResearchStopped(ResearchNode research)
	{
		// TODO
	}

	public void OnResearchFinished(ResearchNode research)
	{
		// TODO
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		ui.UpdateResearchCantBuyText("");
		ui.UpdateResearchDescription("");
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!node.CanBuy() && !researched)
		{
			ui.UpdateResearchCantBuyText("Not enough currency");
		}
		ui.UpdateResearchDescription(node.description);
	}
}
