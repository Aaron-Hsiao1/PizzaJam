using UnityEngine;

public class PlatformButton : MonoBehaviour
{
	[SerializeField] private GameObject platform;

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Boomerrang"))
		{
			platform.SetActive(true);
		}
	}
}
