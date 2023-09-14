using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Campfire : NetworkBehaviour
{
    public Light2D lightGameObject;
    private float timer;

    public GameObject campfire;
    public Sprite[] menuChanges;
    [SerializeField] private GameObject mellowGameObject;
    [SerializeField] private GameObject mage;
    [SerializeField] private GameObject fireBall;
    private int count;
    public bool isLit;
    private bool start;

	private void Awake()
	{
		QualitySettings.vSyncCount = 1;
	}

	void Start()
    {
        start = false;
        isLit = true;
        lightGameObject = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!start)
        {
            if (timer < 0)
            {
                if (isLit) { lightGameObject.falloffIntensity = Random.Range(0.540f, 0.580f); }
                timer = 1;
                count += 1;
                var num = Random.Range(0, 2);
                if (count == 20 && isLit && num == 1)
                {
                    mellowGameObject.SetActive(true);
                    StartCoroutine(BurnMellow());
                }
                else
                {
                    if (count == 20)
                    {
                        isLit = false;
                        StartCoroutine(LightFire());
                    }
                }
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }

        if(start) 
        {
			lightGameObject.pointLightOuterRadius = Mathf.Lerp(lightGameObject.pointLightOuterRadius, 0f, 1.3f * Time.deltaTime); 
            if(lightGameObject.pointLightOuterRadius < .5) { campfire.SetActive(false); }
        }


    }

	IEnumerator StopFire()
    {
        yield return new WaitForSeconds(1);
    }


	IEnumerator BurnMellow()
    {
        mellowGameObject.GetComponent<SpriteRenderer>().sprite = menuChanges[0];
        yield return new WaitForSeconds(2f);
		mellowGameObject.GetComponent<SpriteRenderer>().sprite = menuChanges[1];
		yield return new WaitForSeconds(1f);
		mellowGameObject.GetComponent<SpriteRenderer>().sprite = menuChanges[2];
		yield return new WaitForSeconds(1f);
		mellowGameObject.GetComponent<SpriteRenderer>().sprite = menuChanges[3];
		yield return new WaitForSeconds(1f);
        mellowGameObject.SetActive(false);
        count = 0;
	}

    IEnumerator LightFire()
    {
        campfire.GetComponent<Animator>().SetBool("Dying", true);
		lightGameObject.falloffIntensity = 0.8f;
		yield return new WaitForSeconds(5f);
        mage.GetComponent<SpriteRenderer>().sprite = menuChanges[4];
        Instantiate(fireBall, new Vector3(1.6f, -1.26f, 5.3f), Quaternion.identity);
        yield return new WaitForSeconds(.75f);
		mage.GetComponent<SpriteRenderer>().sprite = menuChanges[5];
        count = 0;

	}


    [ServerRpc]
    public void StartGameServerRPC()
    {
        StartClientRPC();
    }

    [ClientRpc]
    public void StartClientRPC() 
    {
        start = true;

	}
}
