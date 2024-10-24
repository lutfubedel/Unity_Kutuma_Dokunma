using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class MiddleBoxes : MonoBehaviour
{
    float health = 100f;
    public GameObject healthCanvas; 
    public Image healthBar;

    AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }



    [PunRPC]
    public void TakeDamage(float damageSize)
    {
        health -= damageSize;
        healthBar.fillAmount = health / 100f;

        if (health <= 0)
        {
            source.Play();
            PhotonNetwork.Instantiate("destroyEffect", transform.position, Quaternion.identity, 0, null);
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            StartCoroutine(ShowCanvas());
        }
    }



    IEnumerator ShowCanvas()
    {
        if(!healthCanvas.activeInHierarchy)
        {
            healthCanvas.SetActive(true);
            yield return new WaitForSeconds(2);
            healthCanvas.SetActive(false);
        }
    }
}

