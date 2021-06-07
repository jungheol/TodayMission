using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class ReplyTest : MonoBehaviour
{
    string serverUrl = "http://3.35.221.42:3000";
    public Text txtReply;
    public InputField inputComment;
    public HomeScene homeScene;
    public GameObject inputField;
    public GameObject replyText;
    public int completeCount = 0;


    // 댓글 입력 완료 버튼 클릭시 Replytext로 입력
    public void InputCommentBTN()
    {
        StartCoroutine(InputCommentBTNRoutine());
    }

    IEnumerator InputCommentBTNRoutine() {

        // 이메일 패스워드 입력을 서버로 넘겨주기 위해 form 만들기
        WWWForm form = new WWWForm ();
        form.AddField ("email", PlayerPrefs.GetString("email"));
		form.AddField ("text", inputComment.text);
        form.AddField ("mission_id", homeScene.mission_id);
		UnityWebRequest www = UnityWebRequest.Post(serverUrl + "/completeMission", form);

        // 서버 요청 후 응답을 기다림
		yield return www.SendWebRequest();

        // 서버 응답의 에러가 없으면
		if(www.error == null) {
			string responseStr = www.downloadHandler.text;
			JObject response = JObject.Parse(responseStr);
			int code = response["code"].ToObject<int>();

			if(code == 0) {
				// Mission complete 성공
                completeCount = int.Parse(homeScene.completeCountText.text);
                completeCount++;
                homeScene.completeCountText.text = completeCount.ToString();
                txtReply.text = inputComment.text;
                inputField.SetActive(false);
                replyText.SetActive(true);
                
			} else if(code == 1) {
                // Mission complete 실패

				Debug.Log("MissionCompleteBTNRoutine: " + response["msg"].ToString());
			} else {
                // 서버 에러 발생

				Debug.Log("MissionCompleteBTNRoutine: " + response["msg"].ToString());
			}

		} else {
            // 서버 에러가 있으면, 인터넷 연결 문제 또는 서버 다운

			Debug.Log("MissionCompleteBTNRoutine: " + www.error);
		}

        yield return null;
    }
}
