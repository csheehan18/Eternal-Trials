using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private GameObject escMenu;
    [SerializeField] private Button leaveB;
    private bool isActive;

	private void Awake()
	{
		leaveB.onClick.AddListener(() =>
		{
			NetworkManager.Singleton.GetComponent<SteamLobby>().LeaveGame();
		});
	}

	void Start()
    {
        escMenu.SetActive(false);
	}

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isActive) { isActive= false; } else { isActive= true; }
        }

        if(isActive) { escMenu.SetActive(true); } else { escMenu.SetActive(false); }
    }
}
