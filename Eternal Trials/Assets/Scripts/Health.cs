using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class Health : NetworkBehaviour
{
	public NetworkVariable<float> maxHealth = new NetworkVariable<float>();

	public NetworkVariable<float> currentHealth = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	[SerializeField] private TextMesh tm;

	private bool isPoisoned;
	private bool isSlowed;
	private float poisonDamage = 5f;
	private float poisonDuration = 5f;
	private float slowDuration = 5f;

	private Rigidbody2D rb;
	private SpriteRenderer sr;


	private void Start()
	{
		currentHealth.Value = maxHealth.Value;
		isPoisoned = false;
		isSlowed = false;
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
		currentHealth.OnValueChanged += (oldVal, newVal) =>
		{
			if(currentHealth.Value <= 0) { Die(); }
			DisplayClientRPC(currentHealth.Value);
		};
	}

	public void TakeDamage(float damage)
	{
		//if (isPoisoned)
			//damage *= 2; // Double the damage if the character is poisoned

		currentHealth.Value -= damage;
		if(isSlowed && !isPoisoned) { ChangeColorServerRPC(Color.red, Color.blue); }
		if (isPoisoned && !isSlowed) { ChangeColorServerRPC(Color.red, Color.green); }
		if(!isPoisoned&& !isSlowed) { ChangeColorServerRPC(Color.red, Color.white); }
		if(isSlowed && isPoisoned) { ChangeColorServerRPC(Color.red, sr.color); }
	}

	public void ApplyPoison()
	{
		if (!isPoisoned)
		{
			Debug.Log("Poisoned!");
			isPoisoned = true;
			StartCoroutine(PoisonCoroutine());
		}
	}

	private IEnumerator PoisonCoroutine()
	{
		float timer = poisonDuration;
		float tickRate = 1f;
		float tickTimer = tickRate;
		if (isSlowed) { ChangeColorServerRPC(Color.green, Color.blue); } else { ChangeColorServerRPC(Color.green, Color.white); }
		currentHealth.Value -= (int)poisonDamage;
		while (timer > 0f)
		{
			tickTimer -= Time.deltaTime;

			if (tickTimer <= 0f)
			{
				if (isSlowed) { ChangeColorServerRPC(Color.green, Color.blue); } else { ChangeColorServerRPC(Color.green, Color.white); }
				currentHealth.Value -= (int)poisonDamage;
				tickTimer = tickRate;
			}

			timer -= Time.deltaTime;
			yield return null;
		}

		isPoisoned = false;
	}

	public void ApplySlow()
	{
		if (!isSlowed)
		{
			isSlowed = true;
			StartCoroutine(SlowCoroutine());
		}
	}
	
	private IEnumerator SlowCoroutine()
	{
		float originalVelocity = rb.velocity.magnitude;
		rb.velocity *= 0.5f;
		ChangeColorServerRPC(Color.blue, Color.blue);

		yield return new WaitForSeconds(slowDuration);

		ChangeColorServerRPC(Color.blue, Color.white);

		rb.velocity = rb.velocity.normalized * originalVelocity;
		isSlowed = false;
	}

	public void AddHealth(int amount)
	{
		currentHealth.Value += amount;
	}

	[ClientRpc]
	private void DisplayClientRPC(float value)
	{
		tm.text = value.ToString();
	}

	[ServerRpc]
	private void ChangeColorServerRPC(Color color, Color secondColor)
	{
		DisplayColorClientRpc(color, secondColor);
	}

	[ClientRpc]
	private void DisplayColorClientRpc(Color color, Color secondColor)
	{
		StartCoroutine(ShowColor(color, secondColor));
	}

	public void Die()
	{
		currentHealth.Value = maxHealth.Value;
	}

	private IEnumerator ShowColor(Color color, Color secondColor)
	{
		sr.color = color;
		yield return new WaitForSeconds(0.2f);
		sr.color = secondColor;

	}

}
