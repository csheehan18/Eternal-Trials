using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallMenu : MonoBehaviour
{
    Rigidbody2D rb;
    void Start()
    {
        rb= GetComponent<Rigidbody2D>();
		transform.localRotation = Quaternion.Euler(0f, 0, -90f);
	}

	private void Update()
	{
		rb.velocity = new Vector2(1f, 0f).normalized * 5f;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
        if (collision.CompareTag("Campfire"))
        {
            Destroy(this.gameObject);
            collision.GetComponent<Animator>().SetBool("Dying", false);
            collision.GetComponentInChildren<Campfire>().isLit = true;
            collision.GetComponentInChildren<Campfire>().lightGameObject.falloffIntensity = Random.Range(0.540f, 0.580f);
		}
	}
}
