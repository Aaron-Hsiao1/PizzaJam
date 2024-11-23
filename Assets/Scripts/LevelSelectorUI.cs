using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectorUI : MonoBehaviour
{
	[SerializeField] private Button mainMenuButton;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		mainMenuButton.onClick.AddListener(() =>
		{
			SceneManager.LoadScene("Main Menu");
		});
	}
}
