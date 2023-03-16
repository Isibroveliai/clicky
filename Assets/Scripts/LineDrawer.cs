using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
	public Vector3 StartPos { get; set; }
	public Vector3 EndPos { get; set; }

	[SerializeField]
	Color startColor;
	[SerializeField]
	Color unlockedColor;
	[SerializeField]
	Material lineMaterial;

	LineRenderer lr;
	float width = 0.05f;
    void Start()
    {
        lr = GetComponent<LineRenderer>();
		lr.startColor= startColor;
		lr.endColor = startColor;
		lr.material = lineMaterial;
		lr.startWidth = width;
		lr.endWidth = width;

		DrawLine();
	}

   
	void DrawLine()
	{
		lr.SetPosition(0, StartPos);
		lr.SetPosition(1, EndPos);
	}
	public void SetUnlockedColor()
	{
		lr.startColor = unlockedColor;
		lr.endColor = unlockedColor;
		DrawLine();
	}

}
