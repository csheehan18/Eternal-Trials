using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netcode;
using Unity.Netcode;
using TMPro;

public class PlayerStats : NetworkBehaviour
{
	public NetworkVariable<float> Signature = new NetworkVariable<float>();
	public NetworkVariable<float> Damage = new NetworkVariable<float>();
	public NetworkVariable<float> Speed = new NetworkVariable<float>();
	public NetworkVariable<float> FireRate = new NetworkVariable<float>();

	public TMP_Text sigText;

	private void Start()
	{
		if (!IsOwner) { return; }
		sigText = GameObject.FindGameObjectWithTag("Signature").GetComponent<TMP_Text>();

		Signature.OnValueChanged += (oldVal, newVal) =>
		{
			sigText.text= newVal.ToString();
			if(newVal >= 100f) { sigText.color= Color.red; } else { sigText.color= Color.white; }
		};
	}
}
