using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Net.Sockets;

public class LoginManager : MonoBehaviour
{
	public InputField        _idField;
	public InputField        _pwField;
    public LoginServerConfig _config;
	public string            _id;
	public string            _pw;

	public PlayerInfo _info = null;

	private void OnGUI()
	{
		ButtonCheck();
	}

	private void Start()
	{
        #region Network Initiate
        Instantiate(Resources.Load("Prefabs/NetworkManager") as GameObject);
        #endregion

        LoadConfig();
		SetBackground();	
		MakeInputFields();
	}
    
    private void LoadConfig()
    {
        var configText = Resources.Load<TextAsset>("Data/ServerConfig").text;
        _config = LoginServerConfig.CreateFromText(configText);
    }

	private void SetBackground()
	{
		var prefab = Resources.Load("Prefabs/Background") as GameObject;
		var backgroundInstance = Instantiate(prefab).GetComponent<Background>();

		backgroundInstance.Init();
	}

	private void ButtonCheck()
	{
		if (GUI.Button(new Rect((Screen.width / 2) - 155, Screen.height * 2 / 3, 150, 50), "Login"))
		{
			TryLogin(_id, _pw);
		}

		if (GUI.Button(new Rect((Screen.width / 2) + 5, Screen.height * 2 / 3, 150, 50), "Exit"))
		{
			Application.Quit();
		}
	}

	private void MakeInputFields()
	{
		// 초기값 세팅.
		_idField.text = "ID";
		_pwField.text = "Password";

		var idEvent = new InputField.SubmitEvent();
		idEvent.AddListener(GetId);
		_idField.onEndEdit = idEvent;

		var pwEvent = new InputField.SubmitEvent();
		pwEvent.AddListener(GetPw);
		_pwField.onEndEdit = pwEvent;
	}

    // Input Field에 적힌 string을 가져오기위한 메소드.
	private void GetId(string arg0)
	{
		_id = arg0;
	}
	private void GetPw(string arg0)
	{
		_pw = arg0;
	}

	void TryLogin(string id, string pw)
	{
		// Id, Pw 둘 중 하나라도 비어 있으면 안된다.
		if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pw))
		{
			return;
		}

        try
        {
            var request = new HttpPack.LoginReq()
            {
                UserId = id,
                UserPw = pw
            };
            var jsonStr = JsonUtility.ToJson(request);
            var network = FindObjectOfType<NetworkManager>();

            string loginRequestUrl = _config.GetHttpString() + network.GetApiString(LoginApiString.Login); 

            // 로그인 서버로 Post
            network.HttpPost<HttpPack.LoginRes>(loginRequestUrl, jsonStr, HandleLoginMessage);
        }
        catch (UnityException e)
        {
            Debug.Log(e.Message);
        }
	}

    // 로그인 서버에서 온 메시지를 처리해주는 메소드.
    bool HandleLoginMessage(HttpPack.LoginRes response)
    {
        switch ((ErrorCode)response.Result)
        {
            // 정상적으로 처리 된 경우.
            case ErrorCode.None :
                #region LoginProcess
                if (!string.IsNullOrEmpty(response.Token))
                {
                    // 유저 정보에 받은 내용 기록.
                    if (_info == null)
                    {
                        var infoPrefab = Resources.Load("Prefabs/PlayerInfo") as GameObject;
                        _info = Instantiate(infoPrefab).GetComponent<PlayerInfo>();
                    }

                    _info.InfoSetting(_id, response.Token);

                    try
                    {
                        // 받은 내용을 가지고 서버에 로그인 요청.
                        var network = FindObjectOfType<NetworkManager>();
                        var req = new PacketInfo.LoginReq()
                        {
                            _id = _info._id,
                            _token = _info._token
                        };

                        network.SendPacket<PacketInfo.LoginReq>(req, PacketInfo.PacketId.ID_LoginReq);
                    }
                    catch (SocketException e)
                    {
                        Debug.LogAssertion("Socket Send / Receive Error : " + e.Message);
                    }
                }

                break;
                #endregion

            // 아이디나 비밀번호가 잘못된 경우.
            case ErrorCode.LoginUserInfoDontExist :
                Debug.LogAssertion("Invalid Id or Pw");
                break;

            default :
            Debug.LogAssertion("Login Server Error");
                break;
        }

        return true;
    }
}

