using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using TMPro;
using Unity.Netcode;
using Unity.Collections;

public class ShowPlayers : NetworkBehaviour
{
	public PlayerList lobbyManager;
    public TMP_Text playerList;

	List<Color> colors = new List<Color>
	{
		Color.red,
        Color.green,
        Color.blue,
        Color.magenta
    };

	//Runs when a player loads in
	public override void OnNetworkSpawn()
    {
		//Subscribes to if the NetworkList Changes
		lobbyManager.playerNames.OnListChanged += UpdateList;

		//Assign a random color and check if its already chosen and cycle through until its already in the list
		var playerColor = colors[Random.Range(0, colors.Count)];
		while (lobbyManager.playerColors.Contains(playerColor))
		{
			playerColor = colors[Random.Range(0, colors.Count)];
		}

		//Checks for host and adds to playerLists without the need of a serverRPC and subscribes them to OnClientCallBack
		if (IsHost) 
		{
			NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
			ClientListServerRPC(SteamFriends.GetPersonaName(), playerColor);
		}
		else { ClientListServerRPC(SteamFriends.GetPersonaName(), playerColor); }

		base.OnNetworkSpawn();
	}

	[ServerRpc(RequireOwnership =false)]
	public void ClientListServerRPC(string pName, Color pColor)
	{
		lobbyManager.playerColors.Add(pColor);
		lobbyManager.playerNames.Add(pName);
	}

	//Called from MenuHandler when clicking on backButton
	public void LeaveLobby()
	{
		lobbyManager.playerNames.OnListChanged -= UpdateList;
		NetworkManager.Singleton.GetComponent<SteamLobby>().LeaveLobby();
	}


	//Update the list so that it removes who left
	[ServerRpc(RequireOwnership =false)]
	public void LeaveLobbyServerRPC(string pName, Color pColor)
	{
		lobbyManager.playerNames.Remove(pName);
		lobbyManager.playerColors.Remove(pColor);
		Debug.Log($"{pName} has left the game");
	}

	//Called anytime the lobby NetworkList changes!
	//Updates the current list from any changes (Add or Remove)
	public void UpdateList(NetworkListEvent<FixedString64Bytes> changeEvent)
	{
		playerList.text = string.Empty;
		foreach (var player in lobbyManager.playerNames)
		{
			var pColor = ColorUtility.ToHtmlStringRGBA(lobbyManager.playerColors[lobbyManager.playerNames.IndexOf(player)]);
			playerList.text += $"<color=#{pColor}>{player}</color> \n";
		}
	}

	private void OnClientDisconnectCallback(ulong clientId)
	{
		lobbyManager.playerNames.Remove(lobbyManager.playerNames[(int)clientId].ToString());
		lobbyManager.playerColors.Remove(lobbyManager.playerColors[(int)clientId]);
		Debug.Log($"{clientId} was disconnected and was removed from list");
	}


}
