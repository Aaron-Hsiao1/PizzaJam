using UnityEngine;

public class BoomerrangReturn : MonoBehaviour
{
	public GameObject projectile;
	private GameObject player;
	public BoomerrangMech boomerMech;
	public Rigidbody rb;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		player = GameObject.Find("Player");
		boomerMech = player.GetComponent<BoomerrangMech>();
		rb = GetComponent<Rigidbody>();
		boomerMech.goBack = false;
		Invoke(nameof(ReturnThrow), 1);
	}

	// Update is called once per frame
	void Update()
	{

	}
	private void ReturnThrow()
	{
		boomerMech.goBack = true;
		Debug.Log("Going back");
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Obstacle"))
		{
			boomerMech.TriggerShockwave(rb.position);
			Invoke(nameof(ReturnThrow), 0f);
		}
	}
}
