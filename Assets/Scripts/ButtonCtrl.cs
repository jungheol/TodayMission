using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonCtrl : MonoBehaviour
{
    public void RankBTN()
    {
        SceneManager.LoadScene("RankScene");
    }

    public void SettingBTN()
    {
        SceneManager.LoadScene("SettingScene");
    }

    public void HomeBTN()
    {
        SceneManager.LoadScene("HomeScene");
    }

    

}
