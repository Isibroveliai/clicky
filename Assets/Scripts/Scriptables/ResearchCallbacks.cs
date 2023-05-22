using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


/// <summary>
/// Used for research node buy event callbacks
/// 
/// use template:
///		public void Function([int] [float] [string])
///		{
///			//your logic
///		}
///		
/// Attach methods to node scriptable objects
/// </summary>
/// 
public class ResearchCallbacks : MonoBehaviour
{
	GameManager mng;
	private void Start()
	{
		mng = GameManager.instance;
	}

	//CALLBACK METHODS
	public void TestCallback()
	{
		Debug.Log("RESEARCH NODE BOUGHT");
	}
	public void ChangeFurnaceImage(Sprite sprite)
	{
		GameObject furnace = GameObject.Find("/UI/CurrencyGenerator");
		furnace.GetComponent<Image>().sprite = sprite;
		print("CHANGED SPRITE");
	}
}
