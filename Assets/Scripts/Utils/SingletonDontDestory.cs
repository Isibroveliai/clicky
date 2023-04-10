using UnityEngine;

public abstract class SingletonDontDestory<T> : MonoBehaviour where T : class
{
	public static T instance { get; private set; }

	public void Awake()
	{
		if (instance == null)
		{
			instance = this as T;
			DontDestroyOnLoad(this);
		}
		else
		{
			Destroy(this);
		}

		Setup();
	}

	public abstract void Setup();
}



