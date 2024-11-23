using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
	[SerializeField] private Button playButton;

	private void Awake()
	{
		playButton.onClick.AddListener(() =>
		{
			SceneManager.LoadScene("Level Selector");
		});
	}
}
