using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 로직에 필요한 데이터들을 담아두는 클래스.
 * 씬 전환시에 관련 씬 매니저가 할당 해제되어도 기록되어야 할 필요가 있는 정보를 기록한다.
 * 싱글톤으로 접근.
 * 당연히 씬 전환에 사라지지 않는다.
 */
public class DataContainer : MonoSingleton 
{
    // 초기화 메소드.
    // 보유 자료중에 초기화가 필요한 자료가 있다면 여기서 처리.
    public void Initialize()
    {
        LoadConfigs();
    }

    public string            _playerId { get; private set; }
    public string            _playerToken { get; private set; }
    public LoginServerConfig _loginServerConfig { get; private set; }

    // Setters
    public void SetPlayerId   (string playerId)      { _playerId = playerId; }
    public void SetPlayerToken(string receivedToken) { _playerToken = receivedToken; }

    public void LoadConfigs()
    {
        // Login Server Config 로드.
        var loginServerConfigText = Resources.Load<TextAsset>("Data/ServerConfig").text;
        _loginServerConfig = LoginServerConfig.CreateFromText(loginServerConfigText);
    }
}

// 로그인 서버의 스펙을 로드하기 위한 구조체.
public struct LoginServerConfig
{
    [SerializeField]
    public string LoginServerAddr;
    [SerializeField]
    public string Port;

    public static LoginServerConfig CreateFromText(string text)
    {
        LoginServerConfig instance;
        try
        {
#if DEBUG
            instance = new LoginServerConfig()
            {
                LoginServerAddr = "localhost",
                Port = "19000"
            };
#else
            instance = JsonUtility.FromJson<LoginServerConfig>(text);
#endif
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("[Config] Cannot parse Config from source : {0}, Error : {1}", text, e.Message);
            throw;
        }

        return instance;
    }

    // Http Url을 알맞게 뽑아주는 메소드.
    public string GetHttpString()
    {
        var connectString = "http://" + LoginServerAddr + ":" + Port + "/";
        return connectString;
    }
}
