using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    private void Start()
    {
        // Logic Initialize
        Initialize();
        RegistPacketEvents();

        // Ingame Graphic Initialize
        PlatformCreate();
        PlayerCreate();

        // UI Initialize
        UIInitialize();
    }

    private void Update()
    {
        UIUpdate();
    }

    #region PACKET LOGIC

    DataContainer _dataContainer = null;
    NetworkManager _networkManager = null;
    EffectManager _effectManager = null;

    private void Initialize()
    {
        _dataContainer = DataContainer.GetInstance();
        _networkManager = NetworkManager.GetInstance();
        _effectManager = EffectManager.GetInstance();
    }

    private void RegistPacketEvents()
    {
        _networkManager.OnGameStartNotify      += this.OnGameStartNotify;
        _networkManager.OnTurnStartNotify      += this.OnTurnStartNotify;
        _networkManager.OnEnemyTurnStartNotify += this.OnEnemyTurnStartNotify;
        _networkManager.OnMoveAck              += this.OnMoveAck;
        _networkManager.OnEnemyMoveNotify      += this.OnEnemyMoveNotify;
        _networkManager.OnFireAck              += this.OnFireAck;
        _networkManager.OnEnemyFireNotify      += this.OnEnemyFireNotify;
        _networkManager.OnGameSetNotify        += this.OnGameSetNotify;
    }

    // 게임 시작 알림 답변 패킷 처리.
    private void OnGameStartNotify(PacketInfo.GameStartNotify receivedPacket)
    {
        // 받은 패킷에서 내 위치와 상대방 위치를 뽑아옴.
        _player.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(receivedPacket._positionX, 300, 0));
        _dataContainer.SetPlayerPosition(_player.transform.position);

        _enemy.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(receivedPacket._enemyPositionX, 300, 0));
        _dataContainer.SetEnemyPosition(_enemy.transform.position);

        // 플레이어 넘버를 지정.
        if (receivedPacket._playerNumber == 1)
        {
            _playerText.GetComponent<Text>().text = "PLAYER 1";
            _enemyText.GetComponent<Text>().text = "PLAYER 2";
        }
        else
        {
            _playerText.GetComponent<Text>().text = "PLAYER 2";
            _enemyText.GetComponent<Text>().text = "PLAYER 1";
        }

        // 응답을 보내준다.
        var ackPacket = new PacketInfo.GameStartAck()
        {
            _result = (int)ErrorCode.None
        };

        _networkManager.SendPacket(ackPacket, PacketInfo.PacketId.ID_GameStartAck);

        // TODO :: 컷씬이 추가된다면 컷씬 추가 
    }

    // 턴이 시작되었음을 알려주는 패킷 처리.
    private void OnTurnStartNotify(PacketInfo.TurnStartNotify receivedPacket)
    {
        // TODO :: 턴 시작시 바람 얻어와서 적용.

        // TODO :: 내 턴이라는 걸 알려주고 시간 차를 둔 뒤 턴 활성화.
        _player._isMyTurn = true;
    }

    // 상대 턴이 시작되었음을 알려주는 패킷 처리.
    private void OnEnemyTurnStartNotify(PacketInfo.EnemyTurnStartNotify receivedPacket)
    {
        // TODO :: 턴 시작시 바람 얻어와서 적용.

        // TODO :: 상대 턴이라는 걸 알려주고 시간 차를 둔 뒤 턴 활성화.
        _player._isMyTurn = false;
    }

    private void OnMoveAck(PacketInfo.MoveAck receivedPacket)
    {

    }

    private void OnEnemyMoveNotify(PacketInfo.EnemyMoveNotify receivedPacket)
    {
        //_enemy.transform.position = new Vector3(receivedPacket._enemyPositionX / 100f, receivedPacket._enemyPositionY / 100f, 0f);

        _enemy.OnMoveCommand(receivedPacket._enemyPositionX / 100f);
        _dataContainer.SetEnemyPosition(_enemy.transform.position);
    }



    private void OnFireAck(PacketInfo.FireAck receivedPacket)
    {

    }

    private void OnEnemyFireNotify(PacketInfo.EnemyFireNotify receivedPacket)
    {
        // 현재 적군의 포지션과 
    }

    private void OnGameSetNotify(PacketInfo.GameSetNotify receivedPacket)
    {

    }

    private void OnTurnAutoEnd()
    {

    }

    #endregion

    #region INGAME

    private Player _player;
    private Player _enemy;

    private void PlayerCreate()
    {
        // 내 캐릭터 생성.
        var playerSpecText = Resources.Load<TextAsset>("Data/Archer" + (int)_dataContainer._playerType).text;
        var playerSpec = PlayerSpec.CreateFromText(playerSpecText);

        _player = Player.Factory.Create(playerSpec);

        // 적 캐릭터 생성.
        var enemySpecText = Resources.Load<TextAsset>("Data/Archer" + (int)_dataContainer._enemyType).text;
        var enemySpec = PlayerSpec.CreateFromText(enemySpecText);

        _enemy = Player.Factory.Create(enemySpec);
        _enemy.SetEnemy();

        // 이펙트 매니저에서 쓸 수 있도록 등록.
        _effectManager.SetPlayers(_player.gameObject, _enemy.gameObject);
    }

    private void PlatformCreate()
    {
        var platform = Resources.Load<GameObject>("Prefabs/Platform");
        Instantiate(platform);
    }

    #endregion

    #region UI

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

    private void UIUpdate()
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

    #endregion
}
