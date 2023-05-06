using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveFalse : MonoBehaviour
{
	public void OnAnimationEnd()
	{
		print("OnAnimEnd");
		UIManager ui = FindAnyObjectByType<UIManager>();
		
		ui.activeTab.panel.SetActive(false);
		ui.animationEnd = true;

		if(ui.activeTab.flag == UIManager.UITab.flags.SWAP)
		{
			ui.activeTab = ui.nextTab;
		}
		else if(ui.activeTab.flag == UIManager.UITab.flags.NOSWAP)
		{
			ui.activeTab = new UIManager.UITab();
		}
		
		
		//ui.activeTab.Clear();
	}
}
