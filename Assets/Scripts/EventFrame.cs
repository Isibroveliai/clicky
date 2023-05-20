
using System.Collections.Generic;

public class EventFrame
{
	public string description = "<no description>";
	public float researchMultiplier = 1;
	public float clickMultiplier = 1;
	public float generationMultiplier = 1;
	public float energyCap = 0;
	public float energyEfficiency = 0;
	public List<ResearchNode> research;
	public List<Upgrade> upgrades;

	public EventFrame()
	{
		research = new List<ResearchNode>();
		upgrades = new List<Upgrade>();
	}
}

