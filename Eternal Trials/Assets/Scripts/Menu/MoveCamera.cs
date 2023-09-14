using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class MoveCamera : NetworkBehaviour
{
	private bool moveLobby;
	private bool moveMenu;
	public Vector3[] pos;
	public float speed;
	public GameObject backButton;
	public GameObject menu;
	public GameObject lobby;
	public ClassSelection[] bc;

    public void MoveCamLobby()
    {
		menu.SetActive(false);
		moveLobby = true;
    }

	public void MoveCamMenu()
	{
		moveMenu = true;
		foreach (var box in bc) { box.enabled = false; }
		lobby.SetActive(false);
	}

	public void Update()
	{
		if(moveLobby) 
		{
			Camera.main.transform.position = Vector3.MoveTowards(transform.position, pos[0], speed * Time.deltaTime);
			if (Camera.main.transform.position == pos[0] ) 
			{
				foreach (var box in bc) { box.enabled = true; }
				moveLobby = false;
				lobby.SetActive(true); 
			}
		}

		if (moveMenu)
		{
			Camera.main.transform.position = Vector3.MoveTowards(transform.position, pos[1], speed * Time.deltaTime);
			if (Camera.main.transform.position == pos[1]) 
			{
				moveMenu = false; menu.SetActive(true); 
			}
		}
	}
}
