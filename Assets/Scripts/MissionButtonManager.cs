using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionButtonManager : MonoBehaviour
{
    public Text title;
    public Text likes;
    public Text complete;

    public void SetContents(string title, int likes, int complete) {
        this.title.text = title;
        this.likes.text = likes.ToString();
        this.complete.text = complete.ToString();
    }

}
