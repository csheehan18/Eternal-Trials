using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StartButton : NetworkBehaviour
{
    public GameObject startButton;
	public GameObject lobbyManager;

	private bool once;


	private void Start()
	{
		once = true;
	}

	void Update()
	{
		if (!IsHost) { return; }

		if (NetworkManager.IsHost)
		{
			if (NetworkManager.Singleton.ConnectedClients.Count == lobbyManager.GetComponent<PlayerList>().playerPrefabsKey.Count && once)
			{
				startButton.SetActive(true);
				once = false;
			}
			else if (!once && NetworkManager.Singleton.ConnectedClients.Count != lobbyManager.GetComponent<PlayerList>().playerPrefabsKey.Count)
			{
				startButton.SetActive(false);
				once = true;
			}
		}

	}
}
