using UnityEngine;

public class SingletonDontDestory<T> : MonoBehaviour where T : class
{
	public static T instance { get; private set; }

	private void Awake()
	{
		if (instance == null) {
			instance = this as T;
			DontDestroyOnLoad(this);
		} else {
			Destroy(this);
		}
	}
}

