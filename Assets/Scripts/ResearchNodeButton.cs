using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResearchNodeButton : MonoBehaviour
{
	[SerializeField]
	GameObject lineObject;
	[SerializeField]
	ResearchNode node;
	public List<ResearchNodeButton> next;
	public List<ResearchNodeButton> preceding;
	public bool unlocked;

	private Button button;
	private Image image;
	UIManager ui;

	void Start()
    {
		ui = GameObject.Find("/UI").GetComponent<UIManager>();

		unlocked = false;
		image = GetComponent<Image>();
		image.sprite = node.sprite;

		button = GetComponent<Button>();
		button.onClick.AddListener(() => Unlock());

		foreach (var node in next)
		{
			node.GetComponent<Button>().interactable = false;
			node.preceding.Add(this);
			var line = Instantiate(lineObject, transform);
			line.GetComponent<LineDrawer>().StartPos = GetComponent<RectTransform>().position;
			line.GetComponent<LineDrawer>().EndPos = node.GetComponent<RectTransform>().position;
		}
	}

	void Unlock()
	{
		if (!node.CanBuy()) return;

		unlocked = true;
		button.interactable = false;
		foreach(var node in next)
		{
			if(!node.unlocked)
			{
				node.GetComponent<Button>().interactable = true;
			}
			UpdateLines(node);

		}
		UpdateLines(this);
		node.Buy();
	}

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
	public void OnPointerEnter()
	{
		if(!node.CanBuy() && !unlocked)
		{
			ui.UpdateResearchCantBuyText("Not enough currency");
		}
		ui.UpdateResearchDescription(node.description);
	}

	public void OnPointerExit()
	{
		ui.UpdateResearchCantBuyText("");
		ui.UpdateResearchDescription("");
	}






}
