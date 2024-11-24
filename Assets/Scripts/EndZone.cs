using UnityEngine;
using UnityEngine.SceneManagement;

public class EndZone : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player Object"))
		{
			SceneManager.LoadScene("Level Selector");
		}
	}
}
