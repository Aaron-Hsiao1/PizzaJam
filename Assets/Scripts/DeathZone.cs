using UnityEngine;

public class DeathZone : MonoBehaviour
{
	[SerializeField] GameManager gameManager;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player Object"))
		{
			gameManager.KillPlayer();
		}
	}
}
