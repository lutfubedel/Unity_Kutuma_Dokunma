using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager : MonoBehaviour
{

    [Header("Player Health")]
    [SerializeField] private Image player1_HealthBar;
    [SerializeField] private Image player2_HealthBar;

    [SerializeField] private float player1_health;
    [SerializeField] private float player2_health;

    PhotonView pw;

    [Header("Prize Items")]
    [SerializeField] private bool isSpawnerStarted;
    [SerializeField] private int limit;
    [SerializeField] private int ciycleCount;
    [SerializeField] private float waitingTime;
    [SerializeField] private GameObject[] prizeSpawnPoints;

    GameObject player1;
    GameObject player2;

    private void Start()
    {
        isSpawnerStarted = false;
        limit = 5;
        waitingTime = 15f;

        pw = GetComponent<PhotonView>();
    }


    public void MainMenu()
    {
        GameObject.FindWithTag("ServerManager").GetComponent<ServerManager>().usingButton = true;
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();

        PhotonNetwork.LoadLevel(0);
    }

    public void NormalExit()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();

        PhotonNetwork.LoadLevel(0);
    }


    IEnumerator StartPrizeSpawner()
    {
        ciycleCount = 0;
        while (isSpawnerStarted)
        {
            if(limit == ciycleCount)
            {
                isSpawnerStarted = false;
            }

            yield return new WaitForSeconds(waitingTime);
            PhotonNetwork.Instantiate("Prize", prizeSpawnPoints[Random.Range(0, prizeSpawnPoints.Length)].transform.position, Quaternion.identity, 0, null);
            ciycleCount++;
        }
    }


    [PunRPC]
    public void PrizeSpawner()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            isSpawnerStarted = true;
            StartCoroutine(StartPrizeSpawner());
        }
    }

    [PunRPC]
    public void PlayerDamage(int choice, float damageSize)
    {
        if (choice == 1)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                player1_health -= damageSize;
                pw.RPC("UpdateHealthBar", RpcTarget.All, 1, player1_health);

                if (player1_health <= 0)
                {
                    player1 = GameObject.FindWithTag("Player1");
                    player2 = GameObject.FindWithTag("Player2");

                    player1.GetComponent<PhotonView>().RPC("Defeat", RpcTarget.All);
                    player2.GetComponent<PhotonView>().RPC("Victory", RpcTarget.All);

                    pw.RPC("ShowEndGamePanel", RpcTarget.All, player2.GetComponent<PhotonView>().Owner.NickName);
                }
            }
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                player2_health -= damageSize;
                pw.RPC("UpdateHealthBar", RpcTarget.All, 2, player2_health);

                if (player2_health <= 0)
                {
                    player1 = GameObject.FindWithTag("Player1");
                    player2 = GameObject.FindWithTag("Player2");

                    player1.GetComponent<PhotonView>().RPC("Victory", RpcTarget.All);
                    player2.GetComponent<PhotonView>().RPC("Defeat", RpcTarget.All);

                    pw.RPC("ShowEndGamePanel", RpcTarget.All, player1.GetComponent<PhotonView>().Owner.NickName);
                }
            }
        }
    }



    [PunRPC]
    public void HealthPlus(int playerNo)
    {
        if (playerNo == 1)
        {
            player1_health += 30;
            
            if (player1_health >= 100)
            {
                player1_health = 100;
                player1_HealthBar.fillAmount = player1_health / 100;
            }
            else
            {
                player1_HealthBar.fillAmount = player1_health / 100;
            }
        }
        else
        {
            player2_health += 30;
            if (player2_health >= 100)
            {
                player2_health = 100;
                player2_HealthBar.fillAmount = player2_health / 100;
            }
            else
            {
                player2_HealthBar.fillAmount = player2_health / 100;
            }
        }
    }


    [PunRPC]
    public void UpdateHealthBar(int playerNo, float currentHealth)
    {
        if (playerNo == 1)
        {
            player1_health = currentHealth;
            player1_HealthBar.fillAmount = player1_health / 100;
        }
        else
        {
            player2_health = currentHealth;
            player2_HealthBar.fillAmount = player2_health / 100;
        }
    }

    [PunRPC]
    public void ShowEndGamePanel(string winnerName)
    {
        foreach (GameObject item in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (item.CompareTag("Panel_End"))
            {
                item.SetActive(true);
                GameObject.FindWithTag("EndGameText").GetComponent<Text>().text = winnerName + " Wins!";
            }
        }
    }




}


