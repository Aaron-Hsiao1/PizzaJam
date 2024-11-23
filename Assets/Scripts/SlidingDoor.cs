using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
	[SerializeField] private Vector3 endPos;
	[SerializeField] private float speed = 1.0f;

	private bool moving = false;
	private bool opening = true;
	private Vector3 startPos;
	private float delay = 0.0f;

	public bool Moving
	{
		get { return moving; }
		set { moving = value; }
	}

	void Start()
	{
		startPos = transform.position;
	}

	private void FixedUpdate()
	{
		if (moving)
		{
			if (opening)
			{
				MoveDoor(endPos);
			}
			else
			{
				MoveDoor(startPos);
			}
		}
	}

	private void MoveDoor(Vector3 goalPos)
	{
		float dist = Vector3.Distance(transform.position, goalPos);
		if (dist > 0.1)
		{
			transform.position = Vector3.Lerp(transform.position, goalPos, speed * Time.deltaTime);
		}
		else
		{
			if (opening)
			{
				delay += Time.deltaTime;
				if (delay > 1.5f)
				{
					opening = false;
				}
				else
				{
					moving = false;
					opening = true;
				}
			}
		}
	}
}
