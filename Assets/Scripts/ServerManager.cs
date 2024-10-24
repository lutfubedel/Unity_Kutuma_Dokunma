using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class ServerManager : MonoBehaviourPunCallbacks
{
    private TMP_Text serverInfo;

    private Button buttonSaveName;
    private Button buttonFastGame;
    private Button buttonCreateRoom;

    public bool usingButton;

    private void Start()
    {
        serverInfo = GameObject.FindWithTag("ServerInfo").GetComponent<TextMeshProUGUI>();
        
        if (!PlayerPrefs.HasKey("UserName"))
        {
            buttonSaveName = GameObject.FindWithTag("Button_SaveName").GetComponent<Button>();
        }
        else
        {
            buttonFastGame = GameObject.FindWithTag("Button_FastGame").GetComponent<Button>();
            buttonCreateRoom = GameObject.FindWithTag("Button_CreateRoom").GetComponent<Button>();
        }

        PhotonNetwork.ConnectUsingSettings();
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnConnectedToMaster()
    {
        serverInfo.text = "Connected to Server";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        serverInfo.text = "Connected to Lobby";

        if (!PlayerPrefs.HasKey("UserName"))
        {
            buttonSaveName.interactable = true;
        }
        else
        {
            buttonCreateRoom.interactable = true;
            buttonFastGame.interactable = true;
        }
    }

    public void RandomGame()
    {
        PhotonNetwork.LoadLevel(1);
        PhotonNetwork.JoinRandomRoom();
    }

    public void CreateRoomAndJoin()
    {
        PhotonNetwork.LoadLevel(1);
        string roomName = "Room_" + Random.Range(0, 9999999);
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = 2, IsOpen = true, IsVisible = true }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Odaya Girildi");
        InvokeRepeating(nameof(CheckPlayerInfo), 0f,1f);

        GameObject myPlayer = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0, null);
        myPlayer.GetComponent<PhotonView>().Owner.NickName = PlayerPrefs.GetString("UserName");

        if(PhotonNetwork.PlayerList.Length == 2)
        {
            GameObject.FindWithTag("GameManager").GetComponent<PhotonView>().RPC("PrizeSpawner", RpcTarget.All);
        }
    }


    public void CheckPlayerInfo()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (PhotonNetwork.PlayerList.Length == 2)
            {
                GameObject.FindWithTag("PlayersWaiting").SetActive(false);
                GameObject.FindWithTag("Player1Name").GetComponent<TextMeshProUGUI>().text = PhotonNetwork.PlayerList[0].NickName;
                GameObject.FindWithTag("Player2Name").GetComponent<TextMeshProUGUI>().text = PhotonNetwork.PlayerList[1].NickName;

                CancelInvoke(nameof(CheckPlayerInfo));
            }
            else
            {
                GameObject.FindWithTag("PlayersWaiting").SetActive(true);
                GameObject.FindWithTag("Player1Name").GetComponent<TextMeshProUGUI>().text = PhotonNetwork.PlayerList[0].NickName;
                GameObject.FindWithTag("Player2Name").GetComponent<TextMeshProUGUI>().text = ".......";
            }
        }

    }

    public override void OnLeftRoom()
    {
        Debug.Log("Sen Çýktýn");

        if(usingButton)
        {
            Time.timeScale = 1;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Time.timeScale = 1;
            PhotonNetwork.ConnectUsingSettings();

            PlayerPrefs.SetInt("Total_Match", (PlayerPrefs.GetInt("Total_Match") + 1));
            PlayerPrefs.SetInt("Lose", (PlayerPrefs.GetInt("Lose") + 1));
            PlayerPrefs.SetInt("Total_Score", (PlayerPrefs.GetInt("Total_Score") - 150));
        }


    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("Rakip çýktý");

        if (usingButton)
        {
            Time.timeScale = 1;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Time.timeScale = 1;
            PhotonNetwork.ConnectUsingSettings();

            PlayerPrefs.SetInt("Total_Match", (PlayerPrefs.GetInt("Total_Match") + 1));
            PlayerPrefs.SetInt("Win", (PlayerPrefs.GetInt("Win") + 1));
            PlayerPrefs.SetInt("Total_Score", (PlayerPrefs.GetInt("Total_Score") + 150));
        }

        InvokeRepeating(nameof(CheckPlayerInfo), 0f, 1f);
    }





    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
    }



    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        serverInfo.text = "Odaya Girilemedi";
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        serverInfo.text = "Random Bir Odaya Girilemedi";
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        serverInfo.text = "Oda Oluþturulamadý";
    }
}
