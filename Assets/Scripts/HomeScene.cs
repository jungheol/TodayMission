using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class HomeScene : MonoBehaviour
{
    string serverUrl = "http://3.35.221.42:3000";
    public Transform missionContent;
    public Transform commentContent;
    public GameObject missionPanel;
    public Text missionText;
    public Text missionGoodCountText;
    public Text completeCountText;
    public JArray missions;
    public int mission_id;
    public Transform commentList;
    public GameObject myComment;
    public GameObject commentInput;

    // Start is called before the first frame update

    IEnumerator Start()
    {
        // 서버에 요청해서 모든 미션 가져오기
        yield return LoadMissionRoutine();

        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadMissionRoutine() {
        // 날짜 정보를 서버로 넘겨주기 위해 form 만들기
        WWWForm form = new WWWForm ();
		form.AddField ("date", System.DateTime.Now.ToString("yyyy-MM-dd")); // 2021-05-10
		UnityWebRequest www = UnityWebRequest.Post(serverUrl + "/loadMission", form);

        // 서버 요청 후 응답을 기다림
		yield return www.SendWebRequest();

        // 서버 응답의 에러가 없으면
		if(www.error == null) {
			string responseStr = www.downloadHandler.text;
			JObject response = JObject.Parse(responseStr);
			int code = response["code"].ToObject<int>();

			if(code == 0) {
				// 로드 성공
                missions = (JArray)response["data"];

                // 미션 목록 만들기
                for(int i=0; i<missions.Count; i++) {
                    // 게임오브젝트 생성
                    GameObject item = Instantiate(Resources.Load<GameObject>("Prefabs/MissionButton"));

                    // 게임 오브젝트 부모 변경
                    item.transform.SetParent(missionContent, false);

                    int id = missions[i]["id"].ToObject<int>();
                    string title = missions[i]["title"].ToString();
                    int like = missions[i]["likes"].ToObject<int>();
                    int complete = missions[i]["complete"].ToObject<int>();

                    item.GetComponent<MissionButtonManager>().SetContents(title, like, complete);
                        
                    // 버튼 클릭 함수
                    item.GetComponentInChildren<Button>().onClick.AddListener(()=>{
                        missionPanel.SetActive(true);

                        // missionPanel에 내용 채우기 (제목, 좋아요 수, 완료 수)
                        mission_id = id;
                        missionText.text = title;
                        missionGoodCountText.text = like.ToString();
                        completeCountText.text = complete.ToString();

                        // mission에 있는 댓글 가져오기 -> /getComment, mission_id 필요
                        StartCoroutine(GetCommentRoutine());
                    });
                    
                }
                
			} else if(code == 1) {
                // 미션 없음
				Debug.Log("LoadMissionRoutine: " + response["msg"].ToString());
			} else {
                // 서버 에러 발생
				Debug.Log("LoadMissionRoutine: " + response["msg"].ToString());
			}

		} else {
            // 서버 에러가 있으면, 인터넷 연결 문제 또는 서버 다운
			Debug.Log("LoadMissionRoutine: " + www.error);
		}

        yield return null;
    }

    IEnumerator GetCommentRoutine() {
       
        WWWForm form = new WWWForm ();
		form.AddField ("email", PlayerPrefs.GetString("email"));
		form.AddField ("mission_id", mission_id);
		UnityWebRequest www = UnityWebRequest.Post(serverUrl + "/getComment", form);

        // 서버 요청 후 응답을 기다림
		yield return www.SendWebRequest();

        // 서버 응답의 에러가 없으면
		if(www.error == null) {
			string responseStr = www.downloadHandler.text;
			JObject response = JObject.Parse(responseStr);
			int code = response["code"].ToObject<int>();

			if(code == 0) {
				// 로드 성공
                JArray comments = (JArray)response["data"];
                JArray like = (JArray)response["like"];

                // 댓글 목록 만들기
                for(int i=0; i<comments.Count; i++) {
                    int id = comments[i]["id"].ToObject<int>();
                    string email = comments[i]["email"].ToString();
                    string date = comments[i]["createdAt"].ToString();
                    string comment = comments[i]["text"].ToString();

                    if(email.Equals(PlayerPrefs.GetString("email"))) {
                        // 완료로 바꾸고, 내 댓글을 맨 위에 표시해주기
                        commentInput.SetActive(false);
                        myComment.SetActive(true);
                        myComment.GetComponent<CommentTextManager>().SetContents(email, date, comment, true);
                    
                    } else {
                        // 게임오브젝트 생성
                        GameObject item = Instantiate(Resources.Load<GameObject>("Prefabs/CommentText"));

                        // 게임 오브젝트 부모 변경
                        item.transform.SetParent(commentContent, false);

                        // Comment_Text 안에 있는 하트 버튼에 접근하기
                        bool heart = false;
                        for(int j=0; j<like.Count; j++) {
                            int likeId = like[j]["id"].ToObject<int>();
                            
                            if(id == likeId) {
                                heart = true;
                                break;
                            }
                        }

                        item.GetComponent<CommentTextManager>().SetContents(email, date, comment, heart);
                    }
                    
                }
                
			} else if(code == 1) {
                // 댓글 정보 없음
				Debug.Log("GetCommentRoutine: " + response["msg"].ToString());
			} else {
                // 서버 에러 발생
				Debug.Log("GetCommentRoutine: " + response["msg"].ToString());
			}

		} else {
            // 서버 에러가 있으면, 인터넷 연결 문제 또는 서버 다운
			Debug.Log("GetCommentRoutine: " + www.error);
		}

        yield return null;
    }

    public void ClickBackButton() {
        // 초기화

        // 미션 좋아요랑 완료 색깔 지우기
        // 내가 쓴 댓글 초기화
        commentInput.SetActive(true);
        myComment.SetActive(false);
        
        // 댓글 지우기
        foreach(CommentTextManager item in commentList.GetComponentsInChildren<CommentTextManager>()) {
            Destroy(item.gameObject);
        }

        // 화면 전환
        GameObject.Find("Canvas/MissionPanel").SetActive(false);
        GameObject.Find("Canvas/HomePanel").SetActive(true);
    }

}
