using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchNodeButton : MonoBehaviour
{
    public ResearchNode node;
	public List<ResearchNodeButton> neighbors;
	public ResearchNodeButton parent;
	public bool unlocked = false;

	public Image image;

	public Button button;

	public LineRenderer lineRenderer;
	
    void Start()
    {
        image = GetComponent<Image>();
		image.sprite = node.sprite;
		button = GetComponent<Button>();

		lineRenderer = GetComponent<LineRenderer>();
		foreach(var neighbor in neighbors)
		{
			neighbor.GetComponent<Button>().interactable = false;
			neighbor.parent = this;
		}
		button.onClick.AddListener(() => node.Buy());

		
		
    }

	void Update()
	{
		if(parent != null)
		{
			
			lineRenderer.SetPosition(0, this.GetComponent<RectTransform>().position);
			lineRenderer.SetPosition(1, parent.GetComponent<RectTransform>().position);
		}
	}

	

    
   
}
