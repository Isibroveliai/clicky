using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FurnaceButton : MonoBehaviour, IPointerClickHandler
{
	private GameObject particlesObject;
	private ParticleSystem particles;

	// Start is called before the first frame update
	void Start()
    {
		particlesObject = GameObject.Find("Particles");
		particles = particlesObject.GetComponent<ParticleSystem>();
	}

	public void OnPointerClick(PointerEventData pointerEventData)
	{
		var mousePosition = pointerEventData.pointerCurrentRaycast.worldPosition;
		particlesObject.transform.SetPositionAndRotation(mousePosition, particlesObject.transform.rotation);
		particles.Play();
		GameManager.instance.GenerateCurrency();
	}
}
