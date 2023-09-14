using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Steamworks;
using TMPro;

public class MenuHandler : MonoBehaviour
{
	[SerializeField] private Button hostB;
	[SerializeField] private Button joinB;
	[SerializeField] private Button quitB;
	[SerializeField] private Button backB;
	[SerializeField] private Button startB;
	[SerializeField] private Button copyB;
	[SerializeField] private GameObject leave;

	public GameObject Menu;
	public GameObject Lobby;
	public TMP_Text pNames;
	public TMP_InputField lobbyCode;
	public TMP_Text steamLobbyCode;
	public GameObject startGame;

	public GameObject cam;
	public GameObject campFire;

	public SpriteRenderer[] sr;

	public GameObject lobbyManager;

	private void Awake()
	{
		hostB.onClick.AddListener(() =>
		{
			NetworkManager.Singleton.GetComponent<SteamLobby>().HostLobby();
		});

		joinB.onClick.AddListener(() =>
		{
			NetworkManager.Singleton.GetComponent<SteamLobby>().JoinLobby();
		});

		quitB.onClick.AddListener(() =>
		{
			Application.Quit();
		});

		backB.onClick.AddListener(() =>
		{
			//Reset each selected player
			foreach (var r in sr)
			{
				r.color= Color.white;
				r.gameObject.GetComponent<ClassSelection>().isLocal= false;
			}
			lobbyManager.GetComponent<ShowPlayers>().LeaveLobby();
		});

		startB.onClick.AddListener(() =>
		{
			campFire.GetComponent<Campfire>().StartGameServerRPC();
			backB.gameObject.SetActive(false);
			startB.gameObject.SetActive(false);
			NetworkManager.Singleton.GetComponent<LoadPlayers>().StartGame();
		});

		copyB.onClick.AddListener(() =>
		{
			NetworkManager.Singleton.GetComponent<SteamLobby>().CopyID();
		});
	}
}
