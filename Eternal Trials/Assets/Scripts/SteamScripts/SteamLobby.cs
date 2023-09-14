using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using Unity.Netcode;
using Netcode.Transports;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SteamLobby : MonoBehaviour
{
	private MenuHandler mh;
	[SerializeField] private TMP_InputField lobbyCode;
	[SerializeField] private TMP_Text steamLobbyCode;
	private NetworkManager networkManager;

	private const string HostAddressKey = "HostAddress";

	protected Callback<LobbyCreated_t> lobbyCreated;
	protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
	protected Callback<LobbyEnter_t> lobbyEntered;

	private CSteamID currentSteamID;

	private PlayerList playerList;


	void Start()
	{
		networkManager = GetComponent<NetworkManager>();

		if (!SteamManager.Initialized) { return; }

		lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
		gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
		lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

	}

	public void HostLobby()
	{
		SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
	}

	private void OnLobbyCreated(LobbyCreated_t callback)
	{
		if (callback.m_eResult != EResult.k_EResultOK)
		{
			Debug.Log("Cant HOST GAME!");
			return;
		}

		networkManager.StartHost();

		SteamMatchmaking.SetLobbyData(
			new CSteamID(callback.m_ulSteamIDLobby),
			HostAddressKey,
			SteamUser.GetSteamID().ToString());

		currentSteamID = new CSteamID(callback.m_ulSteamIDLobby);

		steamLobbyCode.text = currentSteamID.ToString();

		Debug.Log("Steam Successfully created a lobby: "+ currentSteamID.ToString() +" with Host: "+ SteamFriends.GetPersonaName());
	}

	private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
	{
		SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
	}

	private void OnLobbyEntered(LobbyEnter_t callback)
	{
		if (networkManager.IsHost) { return; }

		string hostAddress = SteamMatchmaking.GetLobbyData(
			new CSteamID(callback.m_ulSteamIDLobby),
			HostAddressKey);

		currentSteamID = new CSteamID(callback.m_ulSteamIDLobby);



		networkManager.GetComponent<SteamNetworkingSocketsTransport>().ConnectToSteamID = (ulong)Convert.ToInt64(hostAddress);
		networkManager.StartClient();

		steamLobbyCode.text = currentSteamID.ToString();

		Camera.main.GetComponent<MoveCamera>().MoveCamLobby();

		Debug.Log("Joined GAME!");
	}

	public void LeaveLobby() 
	{
		SteamMatchmaking.LeaveLobby(currentSteamID);
		NetworkManager.Singleton.Shutdown();
		playerList = GameObject.FindGameObjectWithTag("Lobby").GetComponent<PlayerList>();
		playerList.ClearAllServerRPC();
	}

	public void LeaveGame()
	{
		SteamMatchmaking.LeaveLobby(currentSteamID);
		NetworkManager.Singleton.Shutdown();
		SceneManager.LoadScene("Menu", LoadSceneMode.Single);
		SceneManager.activeSceneChanged += FixGameobjects;
	}

	private void FixGameobjects(Scene current, Scene next)
	{
		if (current.name == null && next.name == "Menu")
		{
			mh = GameObject.Find("MenuHandler").GetComponent<MenuHandler>();
			lobbyCode = mh.lobbyCode;
			steamLobbyCode = mh.steamLobbyCode;
			SceneManager.activeSceneChanged -= FixGameobjects;
			Debug.Log("Finally");
		}
	}

	public void JoinLobby()
	{
		try
		{
			SteamMatchmaking.JoinLobby((CSteamID)Convert.ToUInt64(lobbyCode.text));
		} catch
		{
			Debug.Log("Lobby Does Not Exist");
		}
		
	}

	public void CopyID()
	{
		GUIUtility.systemCopyBuffer = currentSteamID.ToString();
	}

}
