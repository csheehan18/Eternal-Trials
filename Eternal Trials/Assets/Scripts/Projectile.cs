using Steamworks;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float projDamage;
	public float speed;
	[SerializeField] private float range;
	public GameObject player;
	
	public enum ArrowType
	{
		Normal,
		Poison,
		Ice,
		Fire,
	}

	public ArrowType type;
	public float damage;

	private void Awake()
	{
		StartCoroutine(despawn());
	}

	IEnumerator despawn() 
	{
		yield return new WaitForSeconds(range);
		Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.CompareTag("Enemy"))
		{
			if (gameObject.name != NetworkManager.Singleton.LocalClientId.ToString()) { Destroy(gameObject);  return; }
			player.GetComponent<ProjectileHit>().damage = damage;
			player.GetComponent<ProjectileHit>().hitGameObject = collision.gameObject;
			player.GetComponent<ProjectileHit>().Hit(type.ToString());
			Destroy(gameObject);
		}
	}

}
