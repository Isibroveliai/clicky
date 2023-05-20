using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

// TODO: Make this a singleton, so it is acccesible everywhere
public class UIManager : MonoBehaviour
{
	
	GameManager mng;
	[Serializable]
	public class UITab
	{
		public enum flags{
			NOSWAP,
			SWAP
		}
		public GameObject panel; // add reference in editor
		public Button button; // add reference in editor
		[ReadOnly]
		public Button closeButton;
		public Animator anim {get; set;}
		public flags flag;
		private string[] triggers;
		public UITab()
		{		
			triggers = new string[] {"Enable", "Disable"};
		}
		public UITab(GameObject panel, Button button) : this()
		{
			this.panel = panel;
			this.button = button;
		}

		public void ResetTriggers()
		{
			foreach(string trig in triggers)
			{
				anim.ResetTrigger(trig);
			}
		}

	}

	public UITab[] tabs;
	public UITab activeTab;
	public UITab nextTab;

	public GameObject gameOverTab;

	public TMP_Text currencyCounter;
	public TMP_Text energyCounter;
	public TMP_Text researchCounter;

	public Color energySafeColor;
	public Color energyDangerColor;

	[ReadOnly]
	public bool animationEnd;
	private void Start()
	{
		gameOverTab.SetActive(false);
		activeTab = new UITab();
		animationEnd = true;
		foreach (UITab tab in tabs)
		{	
			tab.button.onClick.AddListener(() => { ChangeTab(tab); });
			tab.anim = tab.panel.GetComponent<Animator>();
			//tab.anim.enabled = false;
			tab.panel.SetActive(false);
			tab.closeButton = tab.panel.transform.Find("CloseButton").GetComponent<Button>();
			tab.closeButton.onClick.AddListener(() => {
				animationEnd = false;  
				tab.ResetTriggers();
				tab.flag = UITab.flags.NOSWAP;
				tab.anim.SetTrigger("Disable");
				print("CLOSE BUTTON ON CLICK");
				//tab.ResetTriggers(); 
			});
		}

		mng = GameManager.instance;
		
		//fixes display issue on start when save date is loaded
		UpdateEnergyDisplayDanger(mng.GetEnergyUsage() >= mng.maxEnergy);
		UpdateEnergyDisplay(mng.GetEnergyUsage(), mng.maxEnergy);
		UpdateScoreDisplay(mng.data.currency);
		UpdateResearchSpeedDisplay(mng.researchProduction);
	}

	private void Update() {
		UpdateEnergyDisplay(mng.GetEnergyUsage(), mng.maxEnergy);
	}

	public void SetGameOverShown(bool isShown)
	{
		gameOverTab.SetActive(isShown);
	}

	public void UpdateScoreDisplay(HugeNumber score)
	{
		currencyCounter.text = string.Format("{0} $", score);
	}

	public void UpdateEnergyDisplay(float current, float max)
	{
		energyCounter.text = string.Format("{0:0}/{1:0} kW", current, max);
	}
	public void UpdateEnergyDisplayDanger(bool warning)
	{
		energyCounter.color = warning ? energyDangerColor : energySafeColor;
	}

	public void UpdateResearchSpeedDisplay(float researchSpeed)
	{
		researchCounter.text = researchSpeed.ToString();
	}

	//nelieskit niekas nes galimai sulust bent ka mentai pakeitus :^)
	public void ChangeTab(UITab tab)
	{
		if(!animationEnd) return;
		tab.panel.SetActive(true);
		tab.ResetTriggers();
		nextTab = null;
		
		if(activeTab.panel == null)
		{
			Debug.Log("IS NULL");
			activeTab = tab;
		}
		else if(activeTab != tab)
		{
			animationEnd = false;
			activeTab.ResetTriggers();
			print("SWITCH");
			tab.anim.SetTrigger("Enable");
			activeTab.anim.SetTrigger("Disable");
			activeTab.flag = UITab.flags.SWAP;
			nextTab = tab;
		}
		else
		{
			animationEnd = false;
			activeTab.ResetTriggers();
			print("DISABLE SAME");
			tab.anim.SetTrigger("Disable");
			activeTab.flag = UITab.flags.NOSWAP;
		}
	
	}

	
}
