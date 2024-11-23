using UnityEngine;

public class PlatformButton : MonoBehaviour
{
	[SerializeField] private GameObject platform;
	Animator m_Animator;

    private void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
    }
    private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Boomerrang"))
		{
			platform.SetActive(true);

			m_Animator.SetTrigger("Pressed");
		}
	}
}
