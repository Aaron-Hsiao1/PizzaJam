using UnityEngine;

public class SlidingDoorButton : MonoBehaviour
{
	[SerializeField] private SlidingDoor slidingDoor;
    Animator m_Animator;

    private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Boomerrang") && slidingDoor.Moving == false)
		{
			Debug.Log("collision");
			slidingDoor.Moving = true;
            m_Animator.SetTrigger("Pressed");
        }
	}
}
