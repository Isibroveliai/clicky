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

		button.onClick.AddListener(() =>Unlock());		
		
		
		foreach(var neighbor in neighbors)
		{
			neighbor.GetComponent<Button>().interactable = false;
			neighbor.parent = this;
		}
		
		// lineRenderer = GetComponent<LineRenderer>();
		// lineRenderer.startWidth = 0.1f;
		// lineRenderer.endWidth = 0.1f;
		// Color c1 = Color.white;
    	
		// lineRenderer.startColor = c1;
		// lineRenderer.endColor = c1;
		// lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		
    }

	void Update()
	{
		// if(parent != null)
		// {
		// 	Vector3 pos1 = this.GetComponent<RectTransform>().position;
		// 	Vector3 pos2 = parent.GetComponent<RectTransform>().position;
		// 	lineRenderer.SetPosition(0, new Vector3(pos1.x, pos1.y, 90));
		// 	lineRenderer.SetPosition(1, new Vector3(pos2.x, pos2.y, 90));
		// }
	}
	void Unlock()
	{
		unlocked = true;
		button.interactable = false;
		foreach(var neighbor in neighbors)
		{
			neighbor.GetComponent<Button>().interactable = true;
		}
		node.Buy();
	}
	

	

    
   
}
