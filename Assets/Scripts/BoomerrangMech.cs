using Unity.VisualScripting;
using UnityEngine;

public class BoomerrangMech : MonoBehaviour
{
	[Header("References")]
	public Transform cam;
	public Transform attackPoint;
	public GameObject objectToThrow;
	public GameObject player;
	private GameObject projectile;
	public GameObject holdPrefab;
	public Transform holdSpawn;
	private GameObject holdingBoomer;

	[Header("Throwing")]
	public KeyCode throwKey = KeyCode.Mouse0;
	public float throwForce;
	public float throwUpwardForce;

	private bool boomercreated;
	public bool goBack;

	[Header("SFX")]
	public AudioSource throwSFX;

	[Header("Shockwave")]
	[SerializeField] private float radius = 10f;
	[SerializeField] private float force = 10f;
	[SerializeField] private float upwardModifier = 1f;

	[SerializeField] private Rigidbody rb;
	[SerializeField] private Rigidbody testingRb;

	bool readyToThrow;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		Invoke(nameof(SpawnBoomer), 0f);

		boomercreated = false;
		readyToThrow = true;
		goBack = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(throwKey) && readyToThrow && !boomercreated)
		{
			boomercreated = true;
			Throw();
		}

		if (goBack == true)
		{
			projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, player.transform.position, 50f*Time.deltaTime);
		}

		if (Input.GetKeyDown(KeyCode.U))
		{
			TestForceADd();
		}
	}

	private void Throw()
	{
		readyToThrow = false;
		Destroy(holdingBoomer);
		projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);
		throwSFX.Play();
		//Debug.Log("ball created");
		Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
		Vector3 forceDirection = cam.transform.forward;
		RaycastHit hit;

		if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
		{
			forceDirection = (hit.point - attackPoint.position).normalized;
		}

		Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;
		projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
		//remove later this is just to test
	}

	private void ResetThrow()
	{
		readyToThrow = true;
		Invoke(nameof(SpawnBoomer), 0f);
		//Debug.Log("Resetting");
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag("Boomerrang") && goBack == true)
		{
			Destroy(other.gameObject);
			goBack = false;
			boomercreated = false;
			Invoke(nameof(ResetThrow), 0f);
		}
	}

	private void SpawnBoomer()
	{
		holdingBoomer = Instantiate(holdPrefab, holdSpawn.position, holdSpawn.rotation);
		holdingBoomer.transform.SetParent(holdSpawn.transform, true);
		//Debug.Log("Spawning");
	}

	public void TriggerShockwave(Vector3 position)
	{
		Debug.Log("Shockwave triggered");

		Collider[] colliders = Physics.OverlapSphere(position, radius);
		foreach (Collider collider in colliders)
		{
			Rigidbody rb = collider.GetComponent<Rigidbody>();
			if (rb != null && collider.gameObject.CompareTag("Player"))
			{
				Debug.Log("shockwave actually triggered");
				Debug.Log("rb tag: " + collider.gameObject.name);
				rb = collider.GetComponent<Rigidbody>();
				rb.AddExplosionForce(force, position, radius, upwardModifier, ForceMode.Impulse);
				break;
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, radius);
	}

	private void TestForceADd()
	{
		Debug.Log("forcr added");
		testingRb.AddExplosionForce(force, testingRb.position, radius, upwardModifier, ForceMode.Impulse);
	}

}
