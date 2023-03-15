using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ResearchNodeButton : MonoBehaviour
{
	public GameObject lineObject;
    public ResearchNode node;
	public List<ResearchNodeButton> next;
	public List<ResearchNodeButton> preceding;
	public bool unlocked = false;
	public bool reachable = false;

	public Image image;

	public Button button;

	public LineRenderer lineRenderer;

	private void Awake()
	{
	
	}
	void Start()
    {

		image = GetComponent<Image>();
		image.sprite = node.sprite;
		button = GetComponent<Button>();
		button.onClick.AddListener(() => Unlock());
		foreach (var node in next)
		{
			node.GetComponent<Button>().interactable = false;
			node.reachable = false;
			node.preceding.Add(this);
			var line = Instantiate(lineObject, transform);
			line.GetComponent<LineDrawer>().StartPos = GetComponent<RectTransform>().position;
			line.GetComponent<LineDrawer>().EndPos = node.GetComponent<RectTransform>().position;
		}

		

	}

	void Update()
	{

	}
	void Unlock()
	{
		unlocked = true;
		button.interactable = false;
		foreach(var node in next)
		{
			if(!node.unlocked)
			{
				node.reachable = true;
				node.GetComponent<Button>().interactable = true;
			}
			UpdateLines(node);

		}
		UpdateLines(this);
			//foreach (var node in preceding)
			//{
			//	var obj = Instantiate(lineObject, transform);
			//	obj.GetComponent<LineDrawer>().StartPos = GetComponent<RectTransform>().position;
			//	obj.GetComponent<LineDrawer>().EndPos = node.GetComponent<RectTransform>().position;
			//}
			//int i = 0;
			//foreach(var node in next)
			//{
			//	if(!node.unlocked)
			//	{
			//		node.GetComponent<Button>().interactable = true;
			//	}
			//	UpdateLines(node);
			//	i++;
			//}
			//if (preceding.Count == 0) return;
			//foreach(var node in preceding)
			//{
			//	if(node.unlocked)
			//	UpdateLines(node);

			//}


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
					//node.next.Remove(child);
				}
			}

		}

	}








}
