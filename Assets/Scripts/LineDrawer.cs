using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
	public Vector3 StartPos { get; set; }
	public Vector3 EndPos { get; set; }

	[SerializeField]
	Material lineMaterial;

	
	float width = 0.05f;
    void Start()
    {
		LineRenderer lr = GetComponent<LineRenderer>();
		lr.material = lineMaterial;
		lr.startWidth = width;
		lr.endWidth = width;

		lr.SetPosition(0, StartPos);
		lr.SetPosition(1, EndPos);
	}

	public void UpdateColor(Color color)
	{
		LineRenderer lr = GetComponent<LineRenderer>();
		lr.startColor = color;
		lr.endColor = color;
	}
}
