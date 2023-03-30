using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;

	private AudioSource audioSource;
	private static System.Random rng = new System.Random();
	public AudioClip[] buttonClick;
	public AudioClip[] buyUpgrade;

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(this);
			return;
		}
		instance = this;
		Setup();
	}

	void Setup()
	{
		audioSource = GetComponent<AudioSource>();
	}

	public static void PlayButtonClick()
	{
		PlaySound(instance.buttonClick);
	}

	public static void PlaySound(AudioClip[] clips)
	{
		int n = clips.Length;
		if (n == 0) return;

		var clip = clips[rng.Next() % n];
		instance.audioSource.PlayOneShot(clip);
	}

	public static void PlaySingleSound(AudioClip clip)
	{
		instance.audioSource.PlayOneShot(clip);
	}
}
