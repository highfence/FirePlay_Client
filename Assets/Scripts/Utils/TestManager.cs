using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestManager : MonoBehaviour
{
    public Player _player;
    public Player _enemy;
    public DataContainer _dataContainer;

    private void Awake()
    {
        PlatformCreate();
        PlayerCreate();

        EffectManager.GetInstance().SetPlayers(_player.gameObject, _enemy.gameObject);
        _dataContainer = DataContainer.GetInstance();

        #region TEST

        _dataContainer.SetPlayerId("NADA");

        var testPacket = new PacketInfo.MatchSuccessNotify()
        {
            _gameNumber = 0,
            _enemyId = "ASDF",
            _enemyWins = 0,
            _enemyLoses = 0,
            _enemyType = 2
        };

        _dataContainer.SetGameMatchData(testPacket);

        #endregion

        UIInitialize();

        _playerText.GetComponent<Text>().text = "PLAYER 1";
        _enemyText.GetComponent<Text>().text = "PLAYER 2";
    }

    private void Update()
    {
        // 플레이어 정보가 따라다니도록.
        var playerTextPos = _uiSystem._uiCam.WorldToViewportPoint(_player.transform.position);
        playerTextPos.z = 0;
        playerTextPos.y += 20;
        _playerText.transform.position = playerTextPos;

        var enemyTextPos = _uiSystem._uiCam.WorldToViewportPoint(_enemy.transform.position);
        enemyTextPos.z = 0;
        enemyTextPos.y += 20;
        _enemyText.transform.position = enemyTextPos;
    }

    private UISystem _uiSystem;
    public GameObject _timeText;
    public GameObject _playerHealthBar;
    public GameObject _enemyHealthBar;
    public GameObject _playerNameText;
    public GameObject _enemyNameText;
    public GameObject _playerText;
    public GameObject _playerScoreText;
    public GameObject _enemyText;
    public GameObject _enemyScoreText;

    private GameTimer _gameTimer;

    private void UIInitialize()
    {
        _uiSystem = FindObjectOfType<UISystem>();

        // 시간관련 초기화.
        _timeText = Instantiate(Resources.Load("GUI/TimeText")) as GameObject;
        var timePosition = (new Vector3(Screen.width / 2, Screen.height * 0.95f, 0));
        timePosition.z = 0;
        _timeText.transform.position = timePosition;
        _uiSystem.AttachUI(_timeText);

        _gameTimer = Instantiate(Resources.Load("Prefabs/GameTimer") as GameObject).GetComponent<GameTimer>();
        _gameTimer.SetText(_timeText);
        _gameTimer.OnTurnAutoEnd += OnTurnAutoEnd;

        // 플레이어 체력바 초기화.
        _playerHealthBar = Instantiate(Resources.Load("GUI/HorizontalBoxWithShadow") as GameObject);
        _playerHealthBar.transform.position = new Vector3(Screen.width * 0.2f, Screen.height * 0.88f, 0);
        _uiSystem.AttachUI(_playerHealthBar);

        _player.SetHealthBar(_playerHealthBar);

        // 적군 체력바 초기화.
        _enemyHealthBar = Instantiate(Resources.Load("GUI/HorizontalBoxWithShadow") as GameObject);
        _enemyHealthBar.transform.position = new Vector3(Screen.width * 0.8f, Screen.height * 0.88f, 0);
        _uiSystem.AttachUI(_enemyHealthBar);

        _enemy.SetHealthBar(_enemyHealthBar);

        // 플레이어 정보 초기화.
        _playerNameText = Instantiate(Resources.Load("GUI/PlayerNameText")) as GameObject;
        _playerNameText.GetComponent<Text>().text = _dataContainer._playerId;
        _playerNameText.transform.position = new Vector3(Screen.width * 0.128f, Screen.height * 0.95f, 0);
        _uiSystem.AttachUI(_playerNameText);

        _playerText = Instantiate(Resources.Load("GUI/PlayerText")) as GameObject;
        _uiSystem.AttachUI(_playerText);

        _playerScoreText = Instantiate(Resources.Load("GUI/PlayerScoreText")) as GameObject;
        _playerScoreText.GetComponent<Text>().text = "Wins : " + _dataContainer._playerWins.ToString() + "\n Loses : " + _dataContainer._playerLoses.ToString();
        _playerScoreText.transform.position = new Vector3(Screen.width * 0.40f, Screen.height * 0.95f, 0);
        _uiSystem.AttachUI(_playerScoreText);

        // 적군 정보 초기화.
        _enemyNameText = Instantiate(Resources.Load("GUI/EnemyNameText")) as GameObject;
        _enemyNameText.GetComponent<Text>().text = _dataContainer._enemyId;
        _enemyNameText.transform.position = new Vector3(Screen.width * 0.872f, Screen.height * 0.95f, 0);
        _uiSystem.AttachUI(_enemyNameText);

        _enemyText = Instantiate(Resources.Load("GUI/PlayerText")) as GameObject;
        _uiSystem.AttachUI(_enemyText);

        _enemyScoreText = Instantiate(Resources.Load("GUI/EnemyScoreText")) as GameObject;
        _enemyScoreText.GetComponent<Text>().text = _dataContainer._enemyWins.ToString() + " : Wins \n" + _dataContainer._enemyLoses.ToString() + " : Loses";
        _enemyScoreText.transform.position = new Vector3(Screen.width * 0.60f, Screen.height * 0.95f, 0);
        _uiSystem.AttachUI(_enemyScoreText);

    }

    private void OnTurnAutoEnd()
    {

    }

    private void PlayerCreate()
    {
        // 내 캐릭터 생성.
        var playerSpecText = Resources.Load<TextAsset>("Data/Archer1").text;
        var playerSpec = PlayerSpec.CreateFromText(playerSpecText);

        _player = Player.Factory.Create(playerSpec);
        _player._isMyTurn = true;

        // 적 캐릭터 생성.
        var enemySpecText = Resources.Load<TextAsset>("Data/Archer2").text;
        var enemySpec = PlayerSpec.CreateFromText(enemySpecText);

        _enemy = Player.Factory.Create(enemySpec);
        _enemy.SetEnemy();

        var playerPosition = Camera.main.ScreenToWorldPoint(new Vector3(50, 300, 0));
        playerPosition.z = 10;
        _player.transform.position = playerPosition;

        DataContainer.GetInstance().SetPlayerPosition(_player.transform.position);
        DataContainer.GetInstance().SetEnemyPosition(_enemy.transform.position);
    }

    private void PlatformCreate()
    {
        var platform = Resources.Load<GameObject>("Prefabs/Platform");
        Instantiate(platform);
    }
}
