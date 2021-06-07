using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class RankScene : MonoBehaviour
{
    string serverUrl = "http://3.35.221.42:3000";
    public Transform rankContent;
    public GameObject profilePanel;
    public Image badge1;
    public Text profileText;
    public Text scoreText;
    public int point;

    IEnumerator Start()
    {
        // 서버에 요청해서 모든 미션 가져오기
        yield return LoadRankRoutine();

        yield return null;
    }
    IEnumerator LoadRankRoutine() {
        // 날짜 정보를 서버로 넘겨주기 위해 form 만들기
        WWWForm form = new WWWForm ();
		UnityWebRequest www = UnityWebRequest.Post(serverUrl + "/getRank", form);

        // 서버 요청 후 응답을 기다림
		yield return www.SendWebRequest();

        // 서버 응답의 에러가 없으면
		if(www.error == null) {
			string responseStr = www.downloadHandler.text;
			JObject response = JObject.Parse(responseStr);
			int code = response["code"].ToObject<int>();

			if(code == 0) {
				// 로드 성공
                JArray ranks = (JArray)response["data"];

                // 미션 목록 만들기
                for(int i=0; i<ranks.Count; i++) {
                    // 게임오브젝트 생성
                    GameObject item = Instantiate(Resources.Load<GameObject>("Prefabs/ProfileButton"));

                    // 게임 오브젝트 부모 변경
                    item.transform.SetParent(rankContent, false);

                    int number = (i+1);
                    string name = ranks[i]["name"].ToString();
                    int score = ranks[i]["point"].ToObject<int>();

                    item.GetComponent<ProfileButtonManager>().SetContents(number, name, score);

                    // 버튼 클릭 함수
                    item.GetComponentInChildren<Button>().onClick.AddListener(()=>{
                        profilePanel.SetActive(true);

                        point = score;
                        profileText.text = name;
                        scoreText.text = score.ToString();
                        if (point > 4){
                           badge1.color = new Color(0, 0, 1, 1);
                        }
                    });   
                }
                
			} else if(code == 1) {
                // 랭크 정보 없음
				Debug.Log("LoadRankRoutine: " + response["msg"].ToString());
			} else {
                // 서버 에러 발생
				Debug.Log("LoadRankRoutine: " + response["msg"].ToString());
			}

		} else {
            // 서버 에러가 있으면, 인터넷 연결 문제 또는 서버 다운
			Debug.Log("LoadMissionRoutine: " + www.error);
		}

        yield return null;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ClickBackButton() {
        // 뱃지 색깔 초기화
        badge1.color = new Color(1,1,1,1);

        // 화면 전환
        GameObject.Find("Canvas/ProfilePanel").SetActive(false);
        GameObject.Find("Canvas/RankPanel").SetActive(true);
    }
}
