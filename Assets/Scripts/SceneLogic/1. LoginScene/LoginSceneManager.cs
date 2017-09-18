using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * 로그인 씬 관리자 클래스.
 */
public class LoginSceneManager : MonoBehaviour
{
    #region PACKET LOGIC

    DataContainer _dataContainer;
    NetworkManager _networkManager;

    public void Awake()
    {
        _dataContainer = DataContainer.GetInstance() as DataContainer;
        _networkManager = NetworkManager.GetInstance() as NetworkManager;

        SubscribePacketEvents(_networkManager);
    }

    // 패킷 도착 이벤트 메소드들을 처음에 등록해주는 함수.
    private void SubscribePacketEvents(NetworkManager network)
    {
        network.OnLoginRes += this.OnLoginResArrived;
    }

    // 게임 서버에서 로그인 응답 패킷이 도착했을 경우
    private void OnLoginResArrived(PacketInfo.LoginRes receivedPacket)
    {
        if (receivedPacket._result != (int)ErrorCode.None)
        {
            Debug.LogFormat("Game Server Login Failed, Error Code : {0}", receivedPacket._result);
            return;
        }

        SceneManager.LoadScene("CharacterSelect");
    }

    // 로그인 서버에 로그인 요청을 보내는 함수.
    private bool TryLogin(string writtenId, string writtenPw)
    {
        // id, pw 둘 중 하나라도 적혀있지 않다면 리턴.
        if (string.IsNullOrEmpty(writtenId) || string.IsNullOrEmpty(writtenPw))
        {
            return false;
        }

        try
        {
            // 로그인 서버에 요청.
            var request = new HttpPack.LoginReq()
            {
                UserId = writtenId,
                UserPw = writtenPw
            };
            var jsonBody = JsonUtility.ToJson(request);
            var network = FindObjectOfType<NetworkManager>();

            string loginReqUrl = _dataContainer._loginServerConfig.GetHttpString() + _dataContainer.GetHttpApiString(HttpApiEnum.Login);
            network.HttpPost<HttpPack.LoginRes>(loginReqUrl, jsonBody, OnHttpLoginRes);

            // 적혀있던 패스워드를 저장해놓는다.
            _reqUserId = writtenId;
        }
        catch (UnityException e)
        {
            Debug.Log(e.Message);
            return false;
        }

        return true;
    }

    // 로그인 서버에서 로그인 응답 패킷이 도착했을 경우 처리하는 콜백함수.
    private bool OnHttpLoginRes(HttpPack.LoginRes response)
    {
        switch ((ErrorCode)response.Result)
        {
            // 정상적으로 처리 된 경우.
            case ErrorCode.None:
                #region Error Code : None case
                if (string.IsNullOrEmpty(response.Token))
                {
                    return false;
                }

                // 유저 정보에 받은 토큰 내용 기록.
                _dataContainer.SetPlayerId(_reqUserId);
                _dataContainer.SetPlayerToken(response.Token);

                // 게임서버에 로그인 요청 패킷 보냄.
                var loginReq = new PacketInfo.LoginReq()
                {
                    _id = _dataContainer._playerId,
                    _token = _dataContainer._playerToken
                };

                _networkManager.SendPacket<PacketInfo.LoginReq>(loginReq, PacketInfo.PacketId.ID_LoginReq);

                #endregion
                break;

            // 아이디나 비밀번호가 잘못된 경우.
            case ErrorCode.LoginUserInfoDontExist:
                #region Error Code : User Info Dont Exist 
                // TODO :: 회원가입중이라고 윈도우 띄우기.
                Debug.LogAssertion("Invalid Id or Pw");
                // 가입 요청을 한다.

                var signInReq = new HttpPack.LoginReq()
                {
                    UserId = _id,
                    UserPw = _pw
                };

                var jsonBody = JsonUtility.ToJson(signInReq);
                var signInRequestUrl = _dataContainer._loginServerConfig.GetHttpString() + _dataContainer.GetHttpApiString(HttpApiEnum.SignIn);
                _networkManager.HttpPost<HttpPack.LoginRes>(signInRequestUrl, jsonBody, OnHttpLoginRes);

                #endregion
                break;

            default:
                Debug.Log("Login Server Error");
                break;
        }

        return true;
    }

    #endregion

    #region GUI

    public InputField _idField;
    public InputField _pwField;
    public string _id;
    public string _pw;
    public string _reqUserId;

    private void Start()
    {
        #region MAKE BACKGROUND

        var bgPrefab = Resources.Load("Prefabs/Background") as GameObject;
        var bgInstance = Instantiate(bgPrefab).GetComponent<Background>();

        #endregion

        #region INITIALIZE INPUT FIELDS

        _idField.text = "ID";
        _pwField.text = "Password";

        var idEvent = new InputField.SubmitEvent();
        idEvent.AddListener(GetId);
        _idField.onEndEdit = idEvent;

        var pwEvent = new InputField.SubmitEvent();
        pwEvent.AddListener(GetPw);
        _pwField.onEndEdit = pwEvent;

        #endregion
    }

    private void OnGUI()
    {
        ButtonCheck();
    }

    // 버튼이 눌렸는지 체크해주는 메소드.
    private void ButtonCheck()
    {
        if (GUI.Button(new Rect((Screen.width / 2) - 155, Screen.height * 2 / 3, 150, 50), "Login"))
        {
            TryLogin(_id, _pw);
        }

        // TODO :: 이걸 회원가입 버튼으로 바꿔야 될 것 같은데?
        if (GUI.Button(new Rect((Screen.width / 2) + 5, Screen.height * 2 / 3, 150, 50), "Exit"))
        {
            Application.Quit();
        }
    }

    // Input Field에 적힌 string을 가져오는 콜백 메소드.
    #region INPUT FIELD CALL BACK
    private void GetId(string arg0)
    {
        _id = arg0;
    }

    private void GetPw(string arg0)
    {
        _pw = arg0;
    }
    #endregion

    #endregion
}
