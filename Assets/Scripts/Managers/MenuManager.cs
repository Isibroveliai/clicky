using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
	public int startScene;

	public void StartGame()
	{
		SceneManager.LoadScene(startScene);
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}