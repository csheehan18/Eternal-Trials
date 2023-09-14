using Steamworks;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Mage : NetworkBehaviour
{
	private float currentCoolDownTime;
	private bool isCoolDown;
	[SerializeField] private GameObject[] projectile;
	private GameObject staff;
	private int currentProj;

	public List<Ability> abilities = new List<Ability>();

	private Dictionary<Ability, Coroutine> cooldownCoroutines = new Dictionary<Ability, Coroutine>();
	private Dictionary<Ability, Coroutine> durationCoroutines = new Dictionary<Ability, Coroutine>();

	private bool canShoot;

	private void Start()
	{
		staff = this.gameObject;
		canShoot = true;
		currentProj = 0;
		currentCoolDownTime = GetComponent<PlayerStats>().FireRate.Value;
	}

	void Update()
	{
		if (!IsOwner) { return; }

		if (canShoot)
		{
			if (Input.GetKey(KeyCode.X) && !isCoolDown && GetComponent<PlayerStats>().Signature.Value >= 100f)
			{
				//UseAbility(abilities[3]);
				SignatureUsedServerRPC(NetworkManager.Singleton.LocalClientId);
			}

			if (Input.GetKey(KeyCode.F) && !isCoolDown && !cooldownCoroutines.ContainsKey(abilities[2]))
			{
				//UseAbility(abilities[2]);
			}

			if (Input.GetKey(KeyCode.Q) && !isCoolDown && !cooldownCoroutines.ContainsKey(abilities[1]))
			{
				//UseAbility(abilities[1]);
			}

			if (Input.GetKey(KeyCode.E) && !isCoolDown && !cooldownCoroutines.ContainsKey(abilities[0]))
			{
				//UseAbility(abilities[0]);
			}

			if (Input.GetKey(KeyCode.Mouse0) && !isCoolDown)
			{
				Shoot();
			}
		}

	}

	private void Shoot()
	{
		//staff.GetComponent<Animator>().Play("Shoot", -1, 0f);
		isCoolDown = true;
		StartCoroutine(Reload());
	}

	private void SpawnProjectile(ulong clientId, Quaternion rot, Vector2 direction, int type, Vector3 pos)
	{
		GameObject go = Instantiate(projectile[type], pos, Quaternion.identity);
		go.gameObject.name = clientId.ToString();
		go.gameObject.transform.rotation = rot;
		go.GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x, direction.y).normalized * 20f;
		go.GetComponent<Projectile>().damage = GetDamage() + go.GetComponent<Projectile>().projDamage;
		if (clientId == NetworkManager.Singleton.LocalClientId)
		{
			go.GetComponent<Projectile>().player = this.gameObject;
		}
	}

	[ServerRpc]
	public void ShootServerRPC(ulong clientId, Quaternion rot, Vector2 direction, int arrow, Vector3 pos)
	{
		ShootClientRPC(clientId, rot, direction, arrow, pos);
	}

	[ClientRpc]
	public void ShootClientRPC(ulong clientId, Quaternion rot, Vector2 direction, int arrow, Vector3 pos)
	{
		if (IsOwner) { return; }
		SpawnProjectile(clientId, rot, direction, arrow, pos);
	}


	private IEnumerator Reload()
	{
		AlignProjectile();
		yield return new WaitForSeconds(currentCoolDownTime);
		isCoolDown = false;
	}

	private void AlignProjectile()
	{
		Vector3 mousePos = Input.mousePosition;
		mousePos.z = 0;

		Vector3 objectPos = Camera.main.WorldToScreenPoint(staff.transform.position);
		mousePos.x = mousePos.x - objectPos.x;
		mousePos.y = mousePos.y - objectPos.y;

		float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
		angle -= 90;
		var rot = Quaternion.Euler(new Vector3(0, 0, angle));
		Vector2 direction = (Input.mousePosition - objectPos).normalized;
		ShootServerRPC(NetworkManager.Singleton.LocalClientId, rot, direction, currentProj, staff.transform.position);
		SpawnProjectile(NetworkManager.Singleton.LocalClientId, rot, direction, currentProj, staff.transform.position);
	}

	private float GetDamage()
	{
		if (1 == Random.Range(0, 10))
		{
			return GetComponent<PlayerStats>().Damage.Value * 1.2f;
		}
		else { return GetComponent<PlayerStats>().Damage.Value; }
	}

	private void UseAbility(Ability ability)
	{
		if (cooldownCoroutines.ContainsKey(ability) || durationCoroutines.ContainsKey(ability))
		{
			// Ability is on cooldown or already active
			Debug.Log("Already in use or Cooldown");
			return;
		}

		// Start the cooldown and duration coroutines
		cooldownCoroutines[ability] = StartCoroutine(AbilityCooldownCoroutine(ability));
		durationCoroutines[ability] = StartCoroutine(AbilityDurationCoroutine(ability));

		// Perform the ability's action
		PerformAbilityAction(ability);
	}

	private void PerformAbilityAction(Ability ability)
	{
		if (ability.abilityName == "Summon")
		{
			Debug.Log("Summon!");
		}
		Debug.Log("Ability used: " + ability.abilityName);
	}

	private IEnumerator AbilityCooldownCoroutine(Ability ability)
	{
		yield return new WaitForSeconds(ability.cooldown);

		cooldownCoroutines.Remove(ability);
		Debug.Log("Ability off cooldown: " + ability.abilityName);
	}

	private IEnumerator AbilityDurationCoroutine(Ability ability)
	{
		yield return new WaitForSeconds(ability.duration);

		durationCoroutines.Remove(ability);
		Debug.Log("Ability duration ended: " + ability.abilityName);
	}

	void OnMouseEnter()
	{
		canShoot = false;
	}

	void OnMouseExit()
	{
		canShoot = true;
	}

	[ServerRpc]
	public void SignatureUsedServerRPC(ulong clientId)
	{
		NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject.GetComponent<PlayerStats>().Signature.Value = 0;
	}

}

