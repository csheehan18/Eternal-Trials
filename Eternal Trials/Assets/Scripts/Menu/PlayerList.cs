using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerList : NetworkBehaviour
{
	public NetworkList<FixedString64Bytes> playerNames;
	public NetworkList<ulong> playerPrefabsKey;
	public NetworkList<int> playerPrefabsValue;
	public NetworkList<Color> playerColors;

	public void Start()
	{
		playerNames = new NetworkList<FixedString64Bytes>();
		playerPrefabsKey = new NetworkList<ulong>();
		playerPrefabsValue= new NetworkList<int>();
		playerColors = new NetworkList<Color>();
	}

	[ServerRpc(RequireOwnership = false)]
	public void ClearAllServerRPC()
	{
		playerNames.Clear();
		playerPrefabsKey.Clear();
		playerPrefabsValue.Clear();
		playerColors.Clear();
	}
}
