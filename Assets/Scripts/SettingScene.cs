using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

public class SettingScene : MonoBehaviour
{
    string serverUrl = "http://3.35.221.42:3000";
    public InputField password;
    public InputField passWord;
    public InputField newPassword;
    public InputField passwordCheck;
    public GameObject passwordPanel;
    public GameObject settingPanel;
    public GameObject wdpanel;
     
    // 비밀번호 변경 시 비밀번호 입력 팝업
    public GameObject popup1;
    // 기존 비밀번호 틀렸을 때 팝업
    public GameObject popup2;
    // 서버 통신 장애일 때 팝업
    public GameObject popup3;
    // 새로운 비밀번호 두 개가 같지 않을 때 팝업
    public GameObject popup4;
    // 회원탈퇴 시 비밀번호 입력 팝업
    public GameObject popup5;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PWBTN() {
        popup1.SetActive(true);
    }
    public void PasswordBTN() {
        //비밀번호 변경 시 비밀번호 확인 버튼
        StartCoroutine(PasswordBTNRoutine());
    }
    IEnumerator PasswordBTNRoutine() {

        // 이메일 패스워드 입력을 서버로 넘겨주기 위해 form 만들기
        WWWForm form = new WWWForm ();
        form.AddField ("email", PlayerPrefs.GetString("email"));
		form.AddField ("password", password.text);
		UnityWebRequest www = UnityWebRequest.Post(serverUrl + "/login", form);

        // 서버 요청 후 응답을 기다림
		yield return www.SendWebRequest();

        // 서버 응답의 에러가 없으면
		if(www.error == null) {
			string responseStr = www.downloadHandler.text;
			JObject response = JObject.Parse(responseStr);
			int code = response["code"].ToObject<int>();

			if(code == 0) {
				// 패스워드 인증 성공
                popup1.SetActive(false);
                passwordPanel.SetActive(true);
                
			} else if(code == 1) {
                // 비밀번호 틀린 경우
                popup2.SetActive(true);

				Debug.Log("PasswordBTNRoutine: " + response["msg"].ToString());
			} else {
                // 서버 에러 발생
                popup3.SetActive(true);

				Debug.Log("PasswordBTNRoutine: " + response["msg"].ToString());
			}

		} else {
            // 서버 에러가 있으면, 인터넷 연결 문제 또는 서버 다운
            popup3.SetActive(true);

			Debug.Log("PasswordBTNRoutine: " + www.error);
		}

        yield return null;
    }

    public void ChangePWBTN() {
        StartCoroutine(ChangePWBTNRoutine());
    }

    IEnumerator ChangePWBTNRoutine(){
        
        // 새로 입력한 패스워드가 서로 같지 않을 때 팝업창
        if(newPassword.text != passwordCheck.text) {
            popup4.SetActive(true);
        } else {
        // 패스워드가 일치하면 패스워드 내용을 서버로 보내줌.
        WWWForm form = new WWWForm ();
        form.AddField ("email", PlayerPrefs.GetString("email"));
		form.AddField ("password", newPassword.text);
		UnityWebRequest www = UnityWebRequest.Post(serverUrl + "/changePassword", form);

        // 서버 요청 후 응답을 기다림
		yield return www.SendWebRequest();

        // 서버 응답의 에러가 없으면
		if(www.error == null) {
			string responseStr = www.downloadHandler.text;
			JObject response = JObject.Parse(responseStr);
			int code = response["code"].ToObject<int>();

			if(code == 0) {
                // 비밀번호가 변경되었음.
                passwordPanel.SetActive(false);
                
			} else {
                // 서버 에러 발생
                popup3.SetActive(true);

				Debug.Log("ChangePWBTNRoutine: " + response["msg"].ToString());
			}

		} else {
            // 서버 에러가 있으면, 인터넷 연결 문제 또는 서버 다운
			Debug.Log("ChangePWBTNRoutine: " + www.error);
		}

        yield return null;
        }    
    }

    public void RemoveAccountBTN() {
        popup5.SetActive(true);
    }
    public void PassWordBTN() {
        //회원탈퇴 시 비밀번호 확인 버튼
        StartCoroutine(PassWordBTNRoutine());
    }
    IEnumerator PassWordBTNRoutine() {

        // 이메일 패스워드 입력을 서버로 넘겨주기 위해 form 만들기
        WWWForm form = new WWWForm ();
        form.AddField ("email", PlayerPrefs.GetString("email"));
		form.AddField ("password", passWord.text);
		UnityWebRequest www = UnityWebRequest.Post(serverUrl + "/login", form);

        // 서버 요청 후 응답을 기다림
		yield return www.SendWebRequest();

        // 서버 응답의 에러가 없으면
		if(www.error == null) {
			string responseStr = www.downloadHandler.text;
			JObject response = JObject.Parse(responseStr);
			int code = response["code"].ToObject<int>();

			if(code == 0) {
				// 패스워드 인증 성공
                popup5.SetActive(false);
                wdpanel.SetActive(true);
                
			} else if(code == 1) {
                // 비밀번호 틀린 경우
                popup2.SetActive(true);

				Debug.Log("PassWordBTNRoutine: " + response["msg"].ToString());
			} else {
                // 서버 에러 발생
                popup3.SetActive(true);

				Debug.Log("PassWordBTNRoutine: " + response["msg"].ToString());
			}

		} else {
            // 서버 에러가 있으면, 인터넷 연결 문제 또는 서버 다운
            popup3.SetActive(true);

			Debug.Log("PassWordBTNRoutine: " + www.error);
		}

        yield return null;
    }
    public void RAConfirmBTN() {
        StartCoroutine(RAConfirmBTNRoutine());
    }

    IEnumerator RAConfirmBTNRoutine(){
        
        // 이메일 정보를 서버로 넘겨줌.
        WWWForm form = new WWWForm ();
        form.AddField ("email", PlayerPrefs.GetString("email"));
		UnityWebRequest www = UnityWebRequest.Post(serverUrl + "/removeAccount", form);

        // 서버 요청 후 응답을 기다림
		yield return www.SendWebRequest();

        // 서버 응답의 에러가 없으면
		if(www.error == null) {
			string responseStr = www.downloadHandler.text;
			JObject response = JObject.Parse(responseStr);
			int code = response["code"].ToObject<int>();

			if(code == 0) {
                // 회원탈퇴가 완료되었음.
                wdpanel.SetActive(false);
                
			} else {
                // 서버 에러 발생
                popup3.SetActive(true);

				Debug.Log("RemoveAccountBTNRoutine: " + response["msg"].ToString());
			}

		} else {
            // 서버 에러가 있으면, 인터넷 연결 문제 또는 서버 다운
			Debug.Log("RemoveAccountBTNRoutine: " + www.error);
		}

        yield return null;
    }    
    
    public void PasswordBackBTN()
    {
        passwordPanel.SetActive(false);
        settingPanel.SetActive(true);
    }
    public void WithdrawalBackBTN()
    {
        wdpanel.SetActive(false);
        settingPanel.SetActive(true);
    }

    public void ClickPushToggle(Toggle change) {

        if(change.isOn) {
            // 푸시 알림 온
            // Firebase.Messaging.Subscribe();
            Debug.Log("Push ON");
        } else {
            // 푸시 알림 오프
            // Firebase.Messaging.UnSubscribe();
            Debug.Log("Push OFF");
        }
    }
}
