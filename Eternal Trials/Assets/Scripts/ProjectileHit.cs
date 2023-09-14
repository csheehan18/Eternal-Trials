using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileHit : NetworkBehaviour
{
    public GameObject hitGameObject;
	public float damage;
	public GameObject effect;
	public void Hit(string type)
    {
		HitObjectServerRPC(NetworkManager.Singleton.LocalClientId ,type, hitGameObject, damage);
	}

	[ServerRpc(RequireOwnership =false)]
	public void HitObjectServerRPC(ulong clientId, string type, NetworkObjectReference target, float damage)
	{
		if(NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject.GetComponent<PlayerStats>().Signature.Value < 100f){ NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject.GetComponent<PlayerStats>().Signature.Value += 5; }
		NetworkObject targetObject = target;
		if (type == "Normal") { targetObject.GetComponent<Health>().TakeDamage(damage); Instantiate(effect, targetObject.transform.position, Quaternion.identity); }
		if (type == "Poison") { targetObject.GetComponent<Health>().ApplyPoison(); }
		if (type == "Ice") { targetObject.GetComponent<Health>().ApplySlow(); targetObject.gameObject.GetComponent<Health>().currentHealth.Value -= damage; }
	}

}
