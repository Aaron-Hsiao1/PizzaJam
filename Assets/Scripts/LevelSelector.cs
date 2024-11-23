using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
	public int level;
	public TMP_Text levelText;
	public bool workInProgress;

	private void Start()
	{
		if (workInProgress == false)
		{
			levelText.text = "Level " + level.ToString();
		}
		else
		{
			levelText.text = "WIP";
		}
	}

	public void OpenScene()
	{
		if (workInProgress == true)
		{
			return;
		}
		SceneManager.LoadScene("Level " + level.ToString());
	}
}
