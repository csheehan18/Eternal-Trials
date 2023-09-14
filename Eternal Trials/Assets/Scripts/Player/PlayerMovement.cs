using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
	Rigidbody2D body;

	float horizontal;
	float vertical;
	float moveLimiter = 0.7f;

	private float runSpeed;

	private Animator anim;
	private SpriteRenderer sr;

	void Start()
	{
		runSpeed = GetComponent<PlayerStats>().Speed.Value;
		body = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		sr = GetComponent<SpriteRenderer>();
		this.gameObject.name = OwnerClientId.ToString();
	}

	void Update()
	{
		if(!IsOwner) { return; }
		// Gives a value between -1 and 1
		horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
		vertical = Input.GetAxisRaw("Vertical");	// -1 is down
		if (body.velocity.magnitude > 0)
		{
			anim.SetBool("Moving", true);
		}
		else
		{
			anim.SetBool("Moving", false);
		}

		Camera.main.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -10f);
	}

	void FixedUpdate()
	{
		if(!IsOwner) { return; }

		if (horizontal != 0 && vertical != 0) // Check for diagonal movement
		{
			// limit movement speed diagonally, so you move at 70% speed
			horizontal *= moveLimiter;
			vertical *= moveLimiter;
		}

		Vector2 mouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
		var playerScreenPoint = Camera.main.WorldToScreenPoint(this.transform.position);
		if (mouse.x < playerScreenPoint.x)
		{
			FlipServerRpc(true);
			//left
		}
		else
		{
			FlipServerRpc(false);
			//right
		}
		body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);

	}

	[ServerRpc]
	void FlipServerRpc(bool state)
	{
		UpdateFlipClientRpc(state);
	}

	[ClientRpc]
	void UpdateFlipClientRpc(bool state)
	{
		if(!state) { transform.rotation = Quaternion.Euler(transform.rotation.x, 180, transform.rotation.z); } else { this.gameObject.transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z); }
	}
}
