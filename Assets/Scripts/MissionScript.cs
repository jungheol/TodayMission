using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class MissionScript : MonoBehaviour, IPointerDownHandler
{

    string serverUrl = "http://3.35.221.42:3000";
    public Animator animatorGood;
    public HomeScene homeScene;
    public Text mGoodCountText;
    public int mGoodCount = 0;
    public float DoubleClickSecond = 0.25f;
    private bool mIsOneClick = false;
    private double timer = 0;
    private bool isGood = false;

    private void Start()
    {

    }

    //미션 좋아요 버튼 클릭
    public void MissionGoodBTN()
    {
        StartCoroutine(MissionGoodBTNRoutine());
    }

    IEnumerator MissionGoodBTNRoutine() {

        // 이메일 패스워드 입력을 서버로 넘겨주기 위해 form 만들기
        if(!isGood){
            WWWForm form = new WWWForm ();
            form.AddField ("email", PlayerPrefs.GetString("email"));
            form.AddField ("mission_id", homeScene.mission_id);  // HomeScene에서 해당 mission_id 받아오기.
            UnityWebRequest www = UnityWebRequest.Post(serverUrl + "/likeMission", form);
            
            // 서버 요청 후 응답을 기다림
            yield return www.SendWebRequest();

            // 서버 응답의 에러가 없으면
            if(www.error == null) {
                string responseStr = www.downloadHandler.text;
                JObject response = JObject.Parse(responseStr);
                int code = response["code"].ToObject<int>();

                if(code == 0) {
                    // 미션 좋아요 성공
                    mGoodCount = int.Parse(mGoodCountText.text);
                    mGoodCount++;
                    mGoodCountText.text = mGoodCount.ToString();
                    isGood = true;
                    animatorGood.SetBool("good", true);
                    
                } else if(code == 1) {
                    // 미션 좋아요 실패

                    Debug.Log("MissionGoodBTNRoutine: " + response["msg"].ToString());
                } else {
                    // 서버 에러 발생

                    Debug.Log("MissionGoodBTNRoutine: " + response["msg"].ToString());
                }

            } else {
                // 서버 에러가 있으면, 인터넷 연결 문제 또는 서버 다운
                Debug.Log("MissionGoodBTNRoutine: " + www.error);
            }

            yield return null;

        } else {
            WWWForm form = new WWWForm ();
            form.AddField ("email", PlayerPrefs.GetString("email"));
            form.AddField ("mission_id", homeScene.mission_id);  // HomeScene에서 해당 mission_id 받아오기.
            UnityWebRequest www = UnityWebRequest.Post(serverUrl + "/unlikeMission", form);

            // 서버 요청 후 응답을 기다림
            yield return www.SendWebRequest();

            // 서버 응답의 에러가 없으면
            if(www.error == null) {
                string responseStr = www.downloadHandler.text;
                JObject response = JObject.Parse(responseStr);
                int code = response["code"].ToObject<int>();

                if(code == 0) {
                    // 미션 좋아요 취소 성공
                    mGoodCount = int.Parse(mGoodCountText.text);
                    mGoodCount--;
                    mGoodCountText.text = mGoodCount.ToString();
                    isGood = false;
                    animatorGood.SetBool("good", false);
                    
                } else if(code == 1) {
                    // 미션 좋아요 취소 실패

                    Debug.Log("MissionNoGoodBTNRoutine: " + response["msg"].ToString());
                } else {
                    // 서버 에러 발생

                    Debug.Log("MissionNoGoodBTNRoutine: " + response["msg"].ToString());
                }

            } else {
                // 서버 에러가 있으면, 인터넷 연결 문제 또는 서버 다운
                Debug.Log("MissionNoGoodBTNRoutine: " + www.error);
            }

            yield return null;
        }
    }
    public void OnPointerDown(PointerEventData data)
    {
        if (mIsOneClick && ((Time.time - timer) > DoubleClickSecond))
        {
            Debug.Log("One Click");
            mIsOneClick = false;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            if (!mIsOneClick)
            {
                timer = Time.time;
                mIsOneClick = true;
            }

            else if (!isGood && mIsOneClick && ((Time.time - timer) < DoubleClickSecond))
            { 
                StartCoroutine(MissionGoodBTNRoutine());
            }
            else if (isGood && mIsOneClick && ((Time.time - timer) < DoubleClickSecond))
            {
                StartCoroutine(MissionGoodBTNRoutine());
            }
        }
    }
}
