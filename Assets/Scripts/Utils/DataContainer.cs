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
public class DataContainer : MonoBehaviour
{
    #region SINGLETON

    private static DataContainer _instance = null;

    private void Awake()
    {
        Initialize();
        DontDestroyOnLoad(this.gameObject);
    }

    public static DataContainer GetInstance()
    {
        if (_instance == null)
        {
            _instance = Instantiate(Resources.Load("Prefabs/DataContainer") as GameObject).GetComponent<DataContainer>();
        }
        return _instance;
    }

    // 초기화 메소드.
    // 보유 자료중에 초기화가 필요한 자료가 있다면 여기서 처리.
    public void Initialize()
    {
        LoadConfigs();
        SetDictionary();
    }

    #endregion

    #region DATAS

    // 플레이어 관련 데이터.
    public string        _playerId     { get; private set; }
    public string        _playerToken  { get; private set; }
    public CharacterType _playerType   { get; private set; } 
    public int           _playerWins   { get; private set; }
    public int           _playerLoses  { get; private set; }
    public int           _playerNumber { get; private set; }

    public Vector2       _playerPos    { get; private set; }
    public Vector2       _enemyPos     { get; private set; }

    // 로그인 서버 관련 데이터.
    public LoginServerConfig               _loginServerConfig { get; private set; }
    public Dictionary<HttpApiEnum, string> _httpApiDictionary { get; private set; }

    // 게임 매칭 관련 데이터.
    public int           _gameNumber { get; private set; }
    public string        _enemyId    { get; private set; }
    public int           _enemyWins  { get; private set; }
    public int           _enemyLoses { get; private set; }
    public CharacterType _enemyType  { get; private set; }

    #endregion

    #region SETTERS

    public void SetPlayerId(string playerId) { _playerId = playerId; }
    public void SetPlayerToken(string receivedToken) { _playerToken = receivedToken; }
    public void SetCharacterType(CharacterType type) { _playerType = type; }
    public void SetPlayerScore(int wins, int loses) { _playerWins = wins; _playerLoses = loses; }
    public void SetGameMatchData(PacketInfo.MatchSuccessNotify matchPacket)
    {
        _gameNumber = matchPacket._gameNumber;
        _enemyId = matchPacket._enemyId;
        _enemyWins = matchPacket._enemyWins;
        _enemyLoses = matchPacket._enemyLoses;
        _enemyType = (CharacterType)matchPacket._enemyType;
    }
    public void SetPlayerPosition(Vector2 position) { _playerPos = position; }
    public void SetEnemyPosition(Vector2 position) { _enemyPos = position; }
    public void SetPlayerNumber(int playerNumber) { _playerNumber = playerNumber; }


    #endregion

    #region GETTERS
    public string GetHttpApiString(HttpApiEnum apiEnum)
    {
        string apiString;
        _httpApiDictionary.TryGetValue(apiEnum, out apiString);
        return apiString;
    }
    #endregion

    #region FUNCTIONS

    // 초기에 설정 파일을 로드해주는 메소드.
    private void LoadConfigs()
    {
        // Login Server Config 로드.
        var loginServerConfigText = Resources.Load<TextAsset>("Data/ServerConfig").text;
        _loginServerConfig = LoginServerConfig.CreateFromText(loginServerConfigText);
    }

    // 딕셔너리에 알맞은 값을 넣어주는 메소드.
    private void SetDictionary()
    {
        _httpApiDictionary = new Dictionary<HttpApiEnum, string>()
        {
            { HttpApiEnum.Login, "Request/Login" },
            { HttpApiEnum.SignIn, "Request/SignIn" },
            { HttpApiEnum.Logout, "Request/Logout" }
        };
    }

    #endregion
}

#region DATA STRUCT VARIABLES
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

// 플레이어가 고른 캐릭터 종류 Enum 
public enum CharacterType : int
{
    None = 0,
    Archer1 = 1,
    Archer2 = 2,
    Archer3 = 3
}

#endregion
