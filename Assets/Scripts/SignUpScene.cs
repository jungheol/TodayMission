using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

public class SignUpScene : MonoBehaviour
{
    string serverUrl = "http://3.35.221.42:3000";
    public InputField email;
    public InputField password;
    public InputField passwordCheck;
    public InputField profileName;
    public InputField loginEmail;
    public InputField loginPassword;
    public GameObject profilePanel;
    public GameObject popup1;
    public GameObject popup2;
    public GameObject popup3;
    public GameObject popup4;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SingUpBTN() {
        StartCoroutine(SignUpBTNRoutine());
    }

    IEnumerator SignUpBTNRoutine() {

        if(password.text != passwordCheck.text){
            // 가입 시 입력한 비밀번호 두 개가 같지 않을 때
            Debug.Log("비밀번호를 다시 확인해주세요");
            popup3.SetActive(true);

        } else {
        // 이메일 패스워드 입력을 서버로 넘겨주기 위해 form 만들기
            WWWForm form = new WWWForm ();
            form.AddField ("email", email.text);
            form.AddField ("password", password.text);
            UnityWebRequest www = UnityWebRequest.Post(serverUrl + "/signup", form);

            // 서버 요청 후 응답을 기다림
            yield return www.SendWebRequest();
            
            // 서버 응답의 에러가 없으면
            if(www.error == null) {
                string responseStr = www.downloadHandler.text;
                JObject response = JObject.Parse(responseStr);
                int code = response["code"].ToObject<int>();

                if(code == 0) {
                    // 가입 성공
                    
                    // 가입 또는 로그인된 계정 이메일을 저장해두기 (나중에 필요할때 쓰려고)
                    PlayerPrefs.SetString("email", email.text);

                    profilePanel.SetActive(true);
                    
                } else if(code == 1) {
                    // 이메일 존재하면 팝업
                    popup1.SetActive(true);

                    Debug.Log("SignUpBTNRoutine: " + response["msg"].ToString());

                } else {
                    // 서버 에러 발생
                    popup2.SetActive(true);

                    Debug.Log("SignUpBTNRoutine: " + response["msg"].ToString());
                }

            } else {
                // 서버 에러가 있으면, 인터넷 연결 문제 또는 서버 다운
                popup2.SetActive(true);

                Debug.Log("SignUpBTNRoutine: " + www.error);
            }

            yield return null;
        }
    }

    public void ProfileBTN() {
        StartCoroutine(ProfileBTNRoutine());
    }

    IEnumerator ProfileBTNRoutine(){
        
        WWWForm form = new WWWForm ();
        form.AddField ("email", PlayerPrefs.GetString("email"));
        form.AddField ("name", profileName.text);
		UnityWebRequest www = UnityWebRequest.Post(serverUrl + "/updateName", form);

        // 서버 요청 후 응답을 기다림
		yield return www.SendWebRequest();
        
        // 서버 응답의 에러가 없으면
		if(www.error == null) {
			string responseStr = www.downloadHandler.text;
			JObject response = JObject.Parse(responseStr);
			int code = response["code"].ToObject<int>();

			if(code == 0) {
				// 별명 입력 성공
                
                // 가입 또는 로그인된 계정 별명을 저장해두기 (나중에 필요할때 쓰려고)
                PlayerPrefs.SetString("name", profileName.text);

                SceneManager.LoadScene("HomeScene");
                
			} else if(code == 1) {
                // 별명 존재하면 팝업
                popup4.SetActive(true);

				Debug.Log("SignUpBTNRoutine: " + response["msg"].ToString());
			} else {
                // 서버 에러 발생
                popup2.SetActive(true);

				Debug.Log("SignUpBTNRoutine: " + response["msg"].ToString());
			}

		} else {
            // 서버 에러가 있으면, 인터넷 연결 문제 또는 서버 다운
            popup2.SetActive(true);

			Debug.Log("SignUpBTNRoutine: " + www.error);
		}

        yield return null;
    }


    public void LoginBTN() {
        StartCoroutine(LoginBTNRoutine());
    }

    IEnumerator LoginBTNRoutine() {
        // 이메일 패스워드 입력을 서버로 넘겨주기 위해 form 만들기
        WWWForm form = new WWWForm ();
		form.AddField ("email", loginEmail.text);
		form.AddField ("password", loginPassword.text);
		UnityWebRequest www = UnityWebRequest.Post(serverUrl + "/login", form);

        // 서버 요청 후 응답을 기다림
		yield return www.SendWebRequest();

        // 서버 응답의 에러가 없으면
		if(www.error == null) {
			string responseStr = www.downloadHandler.text;
			JObject response = JObject.Parse(responseStr);
			int code = response["code"].ToObject<int>();

			if(code == 0) {
				// 로그인 성공
                
                // 가입 또는 로그인된 계정 이메일을 저장해두기 (나중에 필요할때 쓰려고)
                PlayerPrefs.SetString("email", loginEmail.text);

                // 홈씬으로 이동
                SceneManager.LoadScene("HomeScene");
                
			} else if(code == 1) {
                // 존재하지 않는 계정 (이메일, 비밀번호 틀린 경우)
                popup3.SetActive(true);

				Debug.Log("LoginBTNRoutine: " + response["msg"].ToString());
			} else {
                // 서버 에러 발생
                popup2.SetActive(true);

				Debug.Log("LoginBTNRoutine: " + response["msg"].ToString());
			}

		} else {
            // 서버 에러가 있으면, 인터넷 연결 문제 또는 서버 다운
			Debug.Log("LoginBTNRoutine: " + www.error);
		}

        yield return null;
    }
}
