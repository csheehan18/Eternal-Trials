using Steamworks;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPlayers : MonoBehaviour
{
    [SerializeField] private GameObject[] playerPrefabs;
	public PlayerList playerList;

	public void StartGame()
    {
		playerList = GameObject.Find("LobbyManager").GetComponent<PlayerList>();
		StartCoroutine(Wait());	
	}

	private void GameLoaded(Scene scene, LoadSceneMode mode)
	{
		if(scene.name == "Game")
		{
			StartGameServerRPC();
			SceneManager.sceneLoaded -= GameLoaded;
		}
	}

	[ServerRpc]
	private void StartGameServerRPC()
    {
		foreach (var playerKey in playerList.playerPrefabsKey)
		{
			Debug.Log(playerKey.ToString());
			GameObject go = Instantiate(playerPrefabs[playerList.playerPrefabsValue[playerList.playerPrefabsKey.IndexOf(playerKey)]], new Vector3(0f, 0f), Quaternion.identity);
			go.gameObject.name = playerList.playerNames[(int)playerKey].ToString();
			go.GetComponent<NetworkObject>().SpawnAsPlayerObject((ulong)(int)playerKey, true);
		}
		NetworkManager.Destroy(playerList.gameObject);
	}

	IEnumerator Wait()
	{
		yield return new WaitForSeconds(3f);
		NetworkManager.DontDestroyOnLoad(playerList.gameObject);
		NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
		SceneManager.sceneLoaded += GameLoaded;
	}
}

