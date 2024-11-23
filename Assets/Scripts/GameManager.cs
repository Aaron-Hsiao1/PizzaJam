using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[Header("Player")]
	[SerializeField] private GameObject player;
	[SerializeField] private GameObject spawnPoint;


	public void KillPlayer()
	{
		player.GetComponentInParent<Rigidbody>().transform.position = spawnPoint.transform.position + new Vector3(0, 1, 0);
		Debug.Log("You Died!");
	}
}
