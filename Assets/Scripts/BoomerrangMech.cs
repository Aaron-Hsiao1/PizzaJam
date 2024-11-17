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
        if(Input.GetKeyDown(throwKey) && readyToThrow && !boomercreated)
        {
            boomercreated = true;
            Throw();
        }

        if (goBack == true)
        {
            projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, player.transform.position, 0.5f);
        }
    }

    private void Throw()
    {
        readyToThrow = false;

        Destroy(holdingBoomer);

        projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);

        Debug.Log("ball created");

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
        Debug.Log("Resetting");
    }

    private void OnTriggerEnter(Collider other)
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
        Debug.Log("Spawning");
    }
}
