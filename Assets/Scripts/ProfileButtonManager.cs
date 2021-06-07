using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileButtonManager : MonoBehaviour
{
    public Text number;
    public Text nikname;
    public Text score;

    public void SetContents(int number, string name, int score) {
        this.number.text = number.ToString();
        this.nikname.text = name;
        this.score.text = score.ToString();
    }
}
