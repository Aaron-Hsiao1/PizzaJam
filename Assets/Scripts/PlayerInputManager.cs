using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System;

public class PlayerInputManager : MonoBehaviour
{
	public static PlayerInputManager Instance { get; private set; }

	public bool escapeMenuOpen;
	public bool canEscape;

	public GameObject escapeMenu;

	public event EventHandler OnInteractAction;
	// Start is called before the first frame update
	void Start()
	{
		escapeMenuOpen = false;
		canEscape = true;

		Cursor.lockState = CursorLockMode.Locked;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && !escapeMenuOpen && canEscape)
		{
			escapeMenu.SetActive(true);
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			escapeMenuOpen = true;
			StartCoroutine(EscapeCD());

		}
		if (Input.GetKeyDown(KeyCode.Escape) && escapeMenuOpen && canEscape)
		{
			escapeMenu.SetActive(false);
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			escapeMenuOpen = false;
			StartCoroutine(EscapeCD());
		}
	}

	IEnumerator EscapeCD()
	{
		canEscape = false;
		yield return new WaitForSecondsRealtime(0.5f);
		canEscape = true;
	}
}
