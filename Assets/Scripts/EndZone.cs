using UnityEngine;
using UnityEngine.SceneManagement;

public class EndZone : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player Object"))
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			SceneManager.LoadScene("Level Selector");
		}
	}
}
