using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResearchTabManager : MonoBehaviour
{
	[SerializeField]
	GameObject line;
	[SerializeField]
	GameObject lineHolder;
	[SerializeField]
	GameObject content;
	UIManager ui;
	GameManager mng;
	public Dictionary<ResearchNodeButton, List<Tuple<ResearchNodeButton, LineDrawer>>> graph; // stores node and its neighbors along with their lines
	
	Dictionary<ResearchNode, ResearchNodeButton> nodeButtonPairs; // used to get ResearchNodeButton that corresponds to ResearchNode
	//private void Awake()
	//{
	//	nodeButtonPairs = new Dictionary<ResearchNode, ResearchNodeButton>();
	//	for (int i = 0; i < content.transform.childCount; i++)
	//	{
	//		GameObject child = content.transform.GetChild(i).gameObject;
	//		ResearchNodeButton node = child.GetComponent<ResearchNodeButton>();
	//		nodeButtonPairs.Add(node.node, node);
	//	}
	//}
	void Start()
    {
		ui = GameObject.Find("/UI").GetComponent<UIManager>();
		mng = GameManager.instance;
		nodeButtonPairs = new Dictionary<ResearchNode, ResearchNodeButton>();

		graph = new Dictionary<ResearchNodeButton, List<Tuple<ResearchNodeButton, LineDrawer>>>();

		//go through each child object of content game object (every button)
		for (int i = 0; i < content.transform.childCount; i++) 
		{
			List<Tuple<ResearchNodeButton, LineDrawer>> neighbors = new List<Tuple<ResearchNodeButton, LineDrawer>>(); // stores a node's neighbors with lines
			GameObject child = content.transform.GetChild(i).gameObject;
			ResearchNodeButton node = child.GetComponent<ResearchNodeButton>();

			nodeButtonPairs.Add(node.node, node); //TODO: change ResearchNode variable in ResearchNodeButton
			child.GetComponent<Button>().interactable = false; //default button state is uninteractable

			foreach (ResearchNodeButton next in node.next)
			{
				LineDrawer drawer;

				//Check all existing registered pairs
				var pair1 = graph.ContainsKey(node) ? graph[node].Find((pair) => pair.Item1 == next) : null;
				var pair2 = graph.ContainsKey(next) ? graph[next].Find((pair) => pair.Item1 == node) : null;
				var pair = pair1 != null ? pair1 : pair2;

				if (pair != null)
				{
					drawer = pair.Item2;
				}
				else // if graph[] doesnt contain both nodes as keys, create a new line object
				{
					GameObject newLine = Instantiate(line, lineHolder.transform);
					drawer = newLine.GetComponent<LineDrawer>();
					drawer.StartPos = node.GetComponent<RectTransform>().position;
					drawer.EndPos = next.GetComponent<RectTransform>().position;
					drawer.UpdateColor(ui.startingLineColor);
				}
				neighbors.Add(new Tuple<ResearchNodeButton, LineDrawer>(next, drawer));
			}
			graph.Add(node, neighbors);

		}
		ResearchNodeButton start = content.transform.Find("Start").GetComponent<ResearchNodeButton>(); // the starting research panel node, unlocks all further research

		start.ChangeButtonState(true);

		GameManager.OnResearchStarted += OnResearchStarted;
		GameManager.OnResearchFinished += OnResearchFinished;
		GameManager.OnResearchStopped += OnResearchStopped;

		LoadButtonStatesFromManager();
	}


	public void OnResearchStarted(ResearchNode research)
	{
		UpdateLines(research, ui.inProgressLineColor);
	}
	public void OnResearchStopped(ResearchNode research)
	{
		UpdateLines(research, ui.startingLineColor);
		nodeButtonPairs[research].ChangeButtonState(true);
	}

	public void OnResearchFinished(ResearchNode research)
	{
		UpdateLines(research, ui.finishedLineColor);
		UnlockNeighbors(nodeButtonPairs[research]);
	}

	/// <summary>
	/// Makes a node's neighbors reachable (unlockable)
	/// </summary>
	/// <param name="node"></param>
	void UnlockNeighbors(ResearchNodeButton node)
	{
		foreach (var neighbor in node.next)
		{
			if (!neighbor.IsResearched()) neighbor.ChangeButtonState(true);
		}
	}
	
	/// <summary>
	/// Update all lines of a node with given color
	/// </summary>
	/// <param name="research"></param>
	/// <param name="color"></param>
	void UpdateLines(ResearchNode research, Color color)
	{
		List<Tuple<ResearchNodeButton, LineDrawer>> neighbors = graph[nodeButtonPairs[research]];

		foreach (var pair in neighbors)
		{
			if (pair.Item1.IsResearched())
			{
				pair.Item2.UpdateColor(color);
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
			UnlockNeighbors(button);
			UpdateLines(button.node, ui.finishedLineColor);
		}
	}
}
