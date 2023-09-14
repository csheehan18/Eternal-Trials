using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ParticleDestroy : NetworkBehaviour
{
    void Start()
    {
        this.gameObject.GetComponent<ParticleSystemRenderer>().material = new Material(Shader.Find("Sprites/Default"));
        Destroy(gameObject);
    }
}
