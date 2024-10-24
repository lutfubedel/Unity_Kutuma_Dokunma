using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ball : MonoBehaviour
{
    [SerializeField] private float damageSize;
    [SerializeField] private int playerNo;
    [SerializeField] private Player player;


    GameManager manager;
    PhotonView pw;
    AudioSource source;

    private void Start()
    {
        manager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        pw = GetComponent<PhotonView>();
        source = GetComponent<AudioSource>();
    }

    [PunRPC]
    public void ImportTag(string tagComing)
    {
        player = GameObject.FindWithTag(tagComing)?.GetComponent<Player>();
        playerNo = (tagComing == "Player1") ? 1 : 2;

    }

    [PunRPC]
    public void DestroyBall()
    {
        if (pw.IsMine)
        {
            PhotonNetwork.Instantiate("hitEffect", transform.position, Quaternion.identity, 0, null);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("MiddleBox"))
        {
            collision.GetComponent<PhotonView>().RPC("TakeDamage",RpcTarget.All, damageSize);
            player.PowerBarPlayAgain();
            source.Play();
            pw.RPC("DestroyBall", RpcTarget.All);
        }

        if (collision.gameObject.CompareTag("Player_1_Tower") || collision.gameObject.CompareTag("Player1"))
        {
            if(playerNo == 2)
            {
                manager.GetComponent<PhotonView>().RPC("PlayerDamage", RpcTarget.All, 1, damageSize);
            }

            player.PowerBarPlayAgain();
            source.Play();
            pw.RPC("DestroyBall", RpcTarget.All);
        }

        if (collision.gameObject.CompareTag("Player_2_Tower") || collision.gameObject.CompareTag("Player2"))
        {
            if(playerNo == 1)
            {
                manager.GetComponent<PhotonView>().RPC("PlayerDamage", RpcTarget.All, 2, damageSize);
            }

            player.PowerBarPlayAgain();
            source.Play();
            pw.RPC("DestroyBall", RpcTarget.All);
        }

        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("FlatBoard"))
        {
            player.PowerBarPlayAgain();
            source.Play();
            pw.RPC("DestroyBall", RpcTarget.All);
        }

        if (collision.gameObject.CompareTag("Prize"))
        {
            manager.GetComponent<PhotonView>().RPC("HealthPlus", RpcTarget.All, playerNo);
            player.PowerBarPlayAgain();
            source.Play();
            PhotonNetwork.Destroy(collision.transform.gameObject);

            pw.RPC("DestroyBall", RpcTarget.All);
        }
    }
}
