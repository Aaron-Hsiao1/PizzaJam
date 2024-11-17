using UnityEngine;

public class SlidingDoorButton : MonoBehaviour
{
	[SerializeField] private SlidingDoor slidingDoor;

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Boomerrang") && slidingDoor.Moving == false)
		{
			Debug.Log("collision");
			slidingDoor.Moving = true;
		}
	}
}
