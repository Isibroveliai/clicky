using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BackgroundParallax : MonoBehaviour
{
	public float animationSpeed = 1;
	public List<Vector2> parralaxSpeeds;

	private List<Vector3> startingPositions;

    void Start()
    {
		startingPositions = new List<Vector3>();
		for (int i = 0; i < transform.childCount; i++)
		{
			var child = transform.GetChild(i);
			var animator = child.GetComponent<Animator>();
			animator.speed = animationSpeed;
			startingPositions.Add(child.transform.position);
		}
	}

	void Update()
	{
		// dx, dy are between [-0.5, 0.5]
		float dx = (Mouse.current.position.x.ReadValue() - (Screen.width / 2)) / Screen.width;
		float dy = (Mouse.current.position.y.ReadValue() - (Screen.height / 2)) / Screen.width;

		for (int i = 0; i < transform.childCount; i++)
		{
			var child = transform.GetChild(i);
			var offset = new Vector3(parralaxSpeeds[i].x * dx, parralaxSpeeds[i].y * dy);
			child.transform.position = startingPositions[i] + offset;
		}
	}
}
