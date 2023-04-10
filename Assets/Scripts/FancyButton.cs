using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FancyButton : MonoBehaviour
{
	private static int pressedOffset = 2;
	private bool isShiftedDown = false;

	private Button button;

	// Start is called before the first frame update
	void Start()
    {
		button = GetComponent<Button>();
    }

	public void OnButtonPressed()
	{
		if (!button.IsInteractable()) return;

		var pos = transform.localPosition;
		transform.localPosition = new Vector3(pos.x, pos.y - pressedOffset, pos.z);
		isShiftedDown = true;
	}

	public void OnButtonReleased()
	{
		if (!isShiftedDown) return;

		var pos = transform.localPosition;
		transform.localPosition = new Vector3(pos.x, pos.y + pressedOffset, pos.z);
		isShiftedDown = false;
	}
}
