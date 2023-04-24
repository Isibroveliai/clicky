using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchTabManager : MonoBehaviour
{
	[SerializeField]
	GameObject line;
	[SerializeField]
	GameObject lineHolder;
	[SerializeField]
	GameObject pages;
	GameManager mng;
	[SerializeField]
	TMP_Text researchDescriptionText;
	[SerializeField]
	TMP_Text researchAdditionalText;
	[SerializeField]
	RectTransform researchProgressbar;
	private float initialProgressbarSize;
	[SerializeField]

	public TMP_Text currentResearchLabel;

	public Dictionary<ResearchNodeButton, List<ResearchNodeButton>> graph; // stores node and its neighbors along with their lines

	Dictionary<ResearchNode, ResearchNodeButton> nodeButtonPairs; // used to get ResearchNodeButton that corresponds to ResearchNode
	GameObject currentPage;
	void Start()
    {
		mng = GameManager.instance;
		nodeButtonPairs = new Dictionary<ResearchNode, ResearchNodeButton>();

		graph = new Dictionary<ResearchNodeButton, List<ResearchNodeButton>>();

		for(int i = 0; i < pages.transform.childCount; i++)
		{
			GameObject page = pages.transform.GetChild(i).gameObject;
			GameObject content = page.transform.Find("Content").gameObject;
			page.SetActive(true);
			//go through each child object of page game object (every button)
			for (int j = 0; j < content.transform.childCount; j++)
			{
				List <ResearchNodeButton> neighbors = new List<ResearchNodeButton>(); // stores a node's neighbors
				GameObject child = content.transform.GetChild(j).gameObject;
				ResearchNodeButton node = child.GetComponent<ResearchNodeButton>();

				nodeButtonPairs.Add(node.node, node); //TODO: change ResearchNode variable in ResearchNodeButton
				child.GetComponent<Button>().interactable = false; //default button state is uninteractable

				foreach (ResearchNodeButton next in node.next)
				{
					neighbors.Add(next);
				}

				graph.Add(node, neighbors);
			}
		}

		ResearchNodeButton start = pages.transform.Find("MainPage/Content/Start").GetComponent<ResearchNodeButton>(); // the starting research panel node, unlocks all further research
		currentPage = GameObject.Find("Pages/MainPage");

		start.ChangeButtonState(true);
		start.node.revealed = true;
		start.ChangeSprite();
		start.OnUnlock();

		GameManager.OnResearchStarted += OnResearchStarted;
		GameManager.OnResearchFinished += OnResearchFinished;
		GameManager.OnResearchStopped += OnResearchStopped;

		LoadButtonStatesFromManager();

		initialProgressbarSize = researchProgressbar.sizeDelta.x;
		UpdateResearchDescription("");
		UpdateCurrentResearchLabel("");
		UpdateResearchProgress(0);

		for (int i = 0; i < pages.transform.childCount; i++)
		{
			GameObject page = pages.transform.GetChild(i).gameObject;
			page.SetActive(false);
		}
		currentPage.SetActive(true);

 	}

	void Update()
	{
		if(mng.activeResearch)
		{
			UpdateResearchProgress(mng.researchPercent);
		}
	}
	public void OnResearchStarted(ResearchNode research)
	{
		UpdateCurrentResearchLabel(research.displayName);
	}
	public void OnResearchStopped(ResearchNode research)
	{
		nodeButtonPairs[research].ChangeButtonState(true);
		UpdateResearchProgress(0);
		UpdateCurrentResearchLabel("");
	}

	public void OnResearchFinished(ResearchNode research)
	{
		UnlockNeighbors(nodeButtonPairs[research]);
		UpdateResearchProgress(0);
		UpdateCurrentResearchLabel("");
	}

	/// <summary>
	/// Makes a node's neighbors reachable (unlockable)
	/// </summary>
	/// <param name="node"></param>
	void UnlockNeighbors(ResearchNodeButton node)
	{
		foreach (var neighbor in node.next)
		{
			if (!neighbor.IsResearched())
			{
				neighbor.ChangeButtonState(true);
				neighbor.OnUnlock();
			} 
		}
	}

	/// <summary>
	/// Loads already unlocked research states
	/// </summary>
	/// <param name="unlockedNodes"></param>
	public void LoadButtonStatesFromManager()
	{
		foreach(string id in mng.data.unlockedResearch)
		{
			var button = nodeButtonPairs.Values.Where(btn => btn.node.id == id).FirstOrDefault();
			if (button == null)
			{
				Debug.Log($"Research button for '{id}' could not be found");
				continue;
			}

			button.ChangeButtonState(false);
			button.OnUnlock();
			UnlockNeighbors(button);
		}
	}
	public void UpdateResearchDescription(string text)
	{
		researchDescriptionText.text = text;
	}
	/// <summary>
	/// Updates additional text of research node description.
	/// </summary>
	/// <param name="text">new text</param>
	/// <param name="append">flag for adding new text to current</param>
	/// <param name="color">new color of text</param>
	public void UpdateResearchAdditionalText(string text, bool append, Color color)
	{
		if(append)
		{
			researchAdditionalText.text = string.Concat(researchAdditionalText.text, text);
			
		}
		else researchAdditionalText.text = text;
		researchAdditionalText.color = color;
		
	}
	public void UpdateResearchProgress(float percent)
	{
		float width = initialProgressbarSize * percent;
		float height = researchProgressbar.sizeDelta.y;
		researchProgressbar.sizeDelta = new Vector2(width, height);
	}

	public void UpdateCurrentResearchLabel(string researchName)
	{
		if (researchName == "") {
			currentResearchLabel.text = "";
		} else {
			currentResearchLabel.text = $"Researching '{researchName}'...";
		}
	}

	public void SwapPages(GameObject newPage)
	{
		currentPage.SetActive(false);
		newPage.SetActive(true);
		currentPage = newPage;
	}
}
