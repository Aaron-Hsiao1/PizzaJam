using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EscapeMenuUI : MonoBehaviour
{
	[SerializeField] private Button quitLevelButton;
	[SerializeField] private Button quitGameButton;

	private void Awake()
	{
		quitLevelButton.onClick.AddListener(() =>
		{
			SceneManager.LoadScene("Level Selector");
		});
		quitGameButton.onClick.AddListener(() =>
		{
			Application.Quit();
		});
	}
}
