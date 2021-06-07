using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstScene : MonoBehaviour
{
    IEnumerator NextScene()
    {
        yield return new WaitForSeconds(2);
        
        if(PlayerPrefs.GetString("email", "") == "") {
            
            SceneManager.LoadScene("SignUpScene");

        } else {
            
            SceneManager.LoadScene("HomeScene");
        }
    }

    private void Start()
    {
        StartCoroutine(NextScene());
    }
}
