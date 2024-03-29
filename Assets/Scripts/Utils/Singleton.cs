﻿using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : class
{
	public static T instance { get; private set; }

	public void Awake()
	{
		if (instance != null && instance != this as T)
		{
			Destroy(this);
			return;
		}
		instance = this as T;

		Setup();
	
	}

	public abstract void Setup();
}

