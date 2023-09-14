using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ClassSelection : NetworkBehaviour
{
	public int prefab;
	private SpriteRenderer sr;
	private ulong localId;
	public PlayerList lobbyManager;
	public bool isLocal;

	private void Start()
	{
		if (IsHost)
		{
			NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
		}
		isLocal = false;
		sr = GetComponent<SpriteRenderer>();
	}

	//Called when we click on the boxCollider2D
	private void OnMouseDown()
	{
		//Sets the local Id for player who clicked, will be used later
		localId = NetworkManager.LocalClientId;

		//Checks to see if the playerList contains the prefab so that two players cant select the same one
		if (!lobbyManager.playerPrefabsKey.Contains(localId))
		{
			//Used to check if its the one we have selected or not
			isLocal= true;
			ClassSelectedServerRPC(localId,prefab);
		}
		else if(isLocal) 
		{
			isLocal= false;
			UnselectClassServerRPC(localId,prefab);
		}
	}


	//Created a key and value to use two networkedLists as a dictionary
	[ServerRpc(RequireOwnership =false)]
	private void ClassSelectedServerRPC(ulong clientId, int prefab)
	{
		lobbyManager.playerPrefabsKey.Add(clientId);
		lobbyManager.playerPrefabsValue.Add(prefab);
		ClassSelectedClientRPC(clientId);
	}

	//Relays back to all clients to change the selected class to the player color and label it as selected
	[ClientRpc]
	private void ClassSelectedClientRPC(ulong clientId) 
	{
		sr.color = lobbyManager.playerColors[(int)clientId];
	}

	//Tells server to remove player if they leave the game
	[ServerRpc(RequireOwnership = false)]
	public void UnselectClassServerRPC(ulong clientId, int prefab)
	{
		lobbyManager.playerPrefabsKey.Remove(clientId);
		lobbyManager.playerPrefabsValue.Remove(prefab);
		UnselectClassClientRPC(clientId);
	}

	//Relays back to players that client has left and they can select class
	[ClientRpc]
	private void UnselectClassClientRPC(ulong clientId)
	{
		sr.color = Color.white;
	}

	private void OnClientDisconnectCallback(ulong clientId)
	{
		Debug.Log($"{clientId} was disconnected");
		UnselectClassServerRPC(clientId, lobbyManager.playerPrefabsValue[lobbyManager.playerPrefabsKey.IndexOf(clientId)]);
	}


}
