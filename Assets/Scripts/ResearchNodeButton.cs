using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class ResearchNodeButton : MonoBehaviour
{
	public GameObject lineObject;
    public ResearchNode node;
	public List<ResearchNodeButton> next;
	public List<ResearchNodeButton> preceding;
	public bool unlocked = false;

	public Image image;

	public Button button;

	public LineRenderer lineRenderer;

	private void Awake()
	{
		image = GetComponent<Image>();
		image.sprite = node.sprite;
		button = GetComponent<Button>();
		button.onClick.AddListener(() => Unlock());
		foreach (var node in next)
		{
			node.GetComponent<Button>().interactable = false;
			node.preceding.Add(this);
		}
	}
	void Start()
    {
        	
		
		
		foreach(var node in preceding)
		{
			var obj = Instantiate(lineObject, transform);
			obj.GetComponent<LineDrawer>().StartPos = node.GetComponent<RectTransform>().position;
			obj.GetComponent<LineDrawer>().EndPos =GetComponent<RectTransform>().position;
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
				node.GetComponent<Button>().interactable = true;
				
			}
		}

		if (preceding.Count == 0) return;
		UpdateLines();
		
		node.Buy();
	}

	void UpdateLines()
	{
		int i = 0;
		foreach(var node in preceding)
		{
			if(node.unlocked)
			{
				var child = transform.GetChild(i);	
				child.GetComponent<LineDrawer>().UpdateColor();
			}
			i++;
			
		}

		foreach(var node in next)
		{
			if(node.unlocked && node.transform.childCount > 0)
			{
				for(int j = 0; j < transform.childCount; j++)
				{
					var child = transform.GetChild(j);
					child.GetComponent<LineDrawer>().UpdateColor();
				}
				
			}
	
		}
	}

	
	

	

    
   
}
