using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Used for research node buy event callbacks
/// 
/// use template:
///		public void Function(BaseEventData eventData)
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
}
