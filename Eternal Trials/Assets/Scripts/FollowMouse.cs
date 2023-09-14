using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FollowMouse : NetworkBehaviour
{
	public GameObject player;

	Vector3 mouse_pos;
	Vector3 object_pos;
	public float angle;

	public Vector2 angleClamp;

	private bool flip;

	void Update()
	{
		if (!IsOwner) { return; }

		mouse_pos = Input.mousePosition;
		mouse_pos.z = 10;
		object_pos = Camera.main.WorldToScreenPoint(player.transform.position);
		mouse_pos.x = mouse_pos.x - object_pos.x;
		mouse_pos.y = mouse_pos.y - object_pos.y;
		angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;

		Vector2 mouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
		var playerScreenPoint = Camera.main.WorldToScreenPoint(player.transform.position);

		if (mouse.x < playerScreenPoint.x)
		{
			if (angle < 0f) angle += 360f;
			angleClamp = new Vector2(90f, 270f);
			if (!flip) { FlipWeaponServerRPC(flip); flip = true; }
			//Left
		}
		else
		{
			angleClamp = new Vector2(-90f, 90f);
			if(flip) { FlipWeaponServerRPC(flip); flip = false; }
			//Right
		}
		var AC = Mathf.Clamp(angle, angleClamp.x, angleClamp.y);

		transform.rotation = Quaternion.Euler(0, 0, AC);

	}


	[ServerRpc]
	public void FlipWeaponServerRPC(bool state)
	{
		FlipWeaponClientRPC(state);
	}

	[ClientRpc]
	public void FlipWeaponClientRPC(bool state)
	{
		if (IsOwner) { return; }
		GetComponent<SpriteRenderer>().flipX = state;
	}
}

