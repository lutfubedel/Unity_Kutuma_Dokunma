using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panel_userName;
    public GameObject panel_entry;
    public GameObject panel_info;

    [Header("UserName UI")]
    public TMP_InputField inputField_userName;
    public TMP_Text userName;

    [Header("Info UI")]
    public TMP_Text text_totalMatch;
    public TMP_Text text_Win;
    public TMP_Text text_Lose;
    public TMP_Text text_totalScore;

    private void Awake()
    {
        if(!PlayerPrefs.HasKey("UserName"))
        {
            panel_userName.SetActive(true);
            panel_entry.SetActive(false);
            panel_info.SetActive(false);

            PlayerPrefs.SetInt("Total_Match", 0);
            PlayerPrefs.SetInt("Win", 0);
            PlayerPrefs.SetInt("Lose", 0);
            PlayerPrefs.SetInt("Total_Score", 0);

            InfoWriter();

        }
        else
        {
            panel_entry.SetActive(true);
            panel_info.SetActive(true);
            panel_userName.SetActive(false);

            userName.text = "username : " + PlayerPrefs.GetString("UserName");

            InfoWriter();
        }
    }

    public void SaveUserName()
    {
        PlayerPrefs.SetString("UserName", inputField_userName.text);

        panel_userName.SetActive(false);
        panel_entry.SetActive(true);
        panel_info.SetActive(true);

        userName.text = "username : " + PlayerPrefs.GetString("UserName");

        GameObject.FindWithTag("Button_FastGame").GetComponent<Button>().interactable = true;
        GameObject.FindWithTag("Button_CreateRoom").GetComponent<Button>().interactable = true;
    }

    public void InfoWriter()
    {
        text_totalMatch.text = PlayerPrefs.GetInt("Total_Match").ToString();
        text_Win.text = PlayerPrefs.GetInt("Win").ToString();
        text_Lose.text = PlayerPrefs.GetInt("Lose").ToString();
        text_totalScore.text = PlayerPrefs.GetInt("Total_Score").ToString();
    }


}
