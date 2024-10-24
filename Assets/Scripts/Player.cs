using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Player : MonoBehaviour
{
    [Header("Ball Items")]
    public GameObject ball;
    public Transform ball_spawnPoint;
    public float floatDirection;

    [Header("Power Bar Items")]
    public Image powerBar;
    public bool powerBar_isOver;

    AudioSource source;
    PhotonView pw;

    public bool canFire;
    private float fireCooldown = 1f;

    void Start()
    {
        source = GetComponent<AudioSource>();
        pw = GetComponent<PhotonView>();

        if (pw.IsMine)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("PlayerTag1 Added");
                pw.RPC("SetPlayerTag", RpcTarget.AllBuffered, "Player1", "SpawnPoint_1", 2f);
            }
            else
            {
                Debug.Log("PlayerTag2 Added");
                pw.RPC("SetPlayerTag", RpcTarget.AllBuffered, "Player2", "SpawnPoint_2", -2f);
            }
        }

        InvokeRepeating(nameof(IsGameStart), 1f, 0.5f);
    }



    [PunRPC]
    public void Victory()
    {
        if(pw.IsMine)
        {
            PlayerPrefs.SetInt("Total_Match", (PlayerPrefs.GetInt("Total_Match") + 1));
            PlayerPrefs.SetInt("Win", (PlayerPrefs.GetInt("Win") + 1));
            PlayerPrefs.SetInt("Total_Score", (PlayerPrefs.GetInt("Total_Score") + 150));
        }

        Time.timeScale = 0;
    }

    [PunRPC]
    public void Defeat()
    {
        if(pw.IsMine)
        {
            PlayerPrefs.SetInt("Total_Match", (PlayerPrefs.GetInt("Total_Match") + 1));
            PlayerPrefs.SetInt("Lose", (PlayerPrefs.GetInt("Lose") + 1));
            PlayerPrefs.SetInt("Total_Score", (PlayerPrefs.GetInt("Total_Score") - 150));
        }
    }

    [PunRPC]
    void SetPlayerTag(string tag, string spawnPointTag, float direction)
    {
        powerBar = GameObject.FindWithTag("PowerBar").GetComponent<Image>();
        gameObject.tag = tag;
        transform.position = GameObject.FindWithTag(spawnPointTag).transform.position;
        transform.rotation = GameObject.FindWithTag(spawnPointTag).transform.rotation;
        floatDirection = direction;
    }


    void Update()
    {
        if (pw.IsMine && canFire)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Fire();
            }
        }
    }





    private void Fire()
    {
        PhotonNetwork.Instantiate("expo_effect", ball_spawnPoint.position, Quaternion.identity, 0, null);
        source.Play();

        GameObject newBall = PhotonNetwork.Instantiate("ball", ball_spawnPoint.position, Quaternion.identity, 0, null);
        Rigidbody2D newBall_rb = newBall.GetComponent<Rigidbody2D>();

        newBall.GetComponent<PhotonView>().RPC("ImportTag", RpcTarget.All, gameObject.tag);
        newBall_rb.AddForce(new Vector2(floatDirection, 0.3f) * powerBar.fillAmount * 12f, ForceMode2D.Impulse);

        StopAllCoroutines();

        canFire = false;
        Invoke(nameof(ChangeCanFire), 1f);
    }
    public void IsGameStart()
    {
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            if (pw.IsMine)
            {
                StartCoroutine(PowerBar());
                CancelInvoke(nameof(IsGameStart));
                canFire = true;
            }
        }
        else
        {
            StopAllCoroutines();
            canFire = false;
        }
    }
    public void PowerBarPlayAgain()
    {
        StartCoroutine(PowerBar());
    }

    public void ChangeCanFire()
    {
        canFire = true;
    }

    IEnumerator FireCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(fireCooldown);
        canFire = true;
    }

    IEnumerator PowerBar()
    {
        powerBar_isOver = false;
        powerBar.fillAmount = 0;
        float value = 0.005f;

        while (true)
        {
            if (powerBar.fillAmount < 1 && !powerBar_isOver)
            {
                powerBar.fillAmount += value;
                yield return new WaitForSecondsRealtime(0.001f * Time.deltaTime);
            }
            else
            {
                powerBar_isOver = true;
                powerBar.fillAmount -= value;
                yield return new WaitForSecondsRealtime(0.001f * Time.deltaTime);

                if (powerBar.fillAmount == 0)
                {
                    powerBar_isOver = false;
                }
            }
        }
    }
}
