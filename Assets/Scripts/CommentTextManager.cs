using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommentTextManager : MonoBehaviour
{
    public Text email;
    public Text date;
    public Text comment;
    public Animator heart;

    public void SetContents(string email, string date, string comment, bool heart) {
        this.email.text = email;
        this.date.text = ConvertDate(date);
        this.comment.text = comment;
        this.heart.SetBool("good", heart);
    }

    string ConvertDate(string date) {
        string newDate = date;

        // 원하는 날짜 형식으로 바꿔서 리턴

        return newDate;
    }
}
