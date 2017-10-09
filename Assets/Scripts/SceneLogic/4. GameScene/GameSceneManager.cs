using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PacketInfo;
using UnityEngine.SceneManagement;
using DigitalRuby.Tween;

public class GameSceneManager : MonoBehaviour
{
    private IEnumerator Start()
    {
        // Logic Initialize
        Initialize();

        // Ingame Graphic Initialize
        yield return PlatformCreate();
        yield return PlayerCreate();

        RegistPacketEvents();

        // UI Initialize
        UIInitialize();

        // 게임을 시작할 준비가 되었음을 서버에 알린다.
        SendMatchSuccessAck();
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
    private void OnGameStartNotify(GameStartNotify receivedPacket)
    {
        // 받은 패킷에서 내 위치와 상대방 위치를 뽑아옴.
        var initPlayerPosition =  new Vector3(receivedPacket._positionX, 30, 0);
        _player.transform.position = initPlayerPosition;
        _dataContainer.SetPlayerPosition(_player.transform.position);

        var initEnemyPosition =  new Vector3(receivedPacket._enemyPositionX, 30, 0);
        _enemy.transform.position = initEnemyPosition;
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

        _dataContainer.SetPlayerNumber(receivedPacket._playerNumber);

        // 응답을 보내준다.
        var ackPacket = new PacketInfo.GameStartAck()
        {
            _result = (int)ErrorCode.None
        };

        _networkManager.SendPacket(ackPacket, PacketInfo.PacketId.ID_GameStartAck);

        // TODO :: 컷씬이 추가된다면 컷씬 추가 
    }

    // 턴이 시작되었음을 알려주는 패킷 처리.
    private void OnTurnStartNotify(TurnStartNotify receivedPacket)
    {
        // TODO :: 턴 시작시 바람 얻어와서 적용.
        _gameTimer.TurnStart();
        StartCoroutine("OnTurnChanged", false);
    }

    // 상대 턴이 시작되었음을 알려주는 패킷 처리.
    private void OnEnemyTurnStartNotify(EnemyTurnStartNotify receivedPacket)
    {
        // TODO :: 턴 시작시 바람 얻어와서 적용.
        _gameTimer.TurnStart();
        _player._isMyTurn = false;
        StartCoroutine("OnTurnChanged", true);
    }

    // 서버가 움직임을 확인했음을 알려주는 패킷 처리.
    private void OnMoveAck(MoveAck receivedPacket)
    {
        
    }

    // 상대의 움직임을 알려주는 패킷 처리.
    private void OnEnemyMoveNotify(EnemyMoveNotify receivedPacket)
    {
        _enemy.StartCoroutine("OnMoveCommanded", receivedPacket._enemyPositionX);
        _dataContainer.SetEnemyPosition(_enemy.transform.position);
    }

    // 서버가 발사를 확인했음을 알려주는 패킷 처리.
    private void OnFireAck(FireAck receivedPacket)
    {

    }

    // 상대의 발사를 알려주는 패킷 처리.
    private void OnEnemyFireNotify(EnemyFireNotify receivedPacket)
    {
        // 현재 적군의 위치가 잘 동기화 되었는지 확인.
        if (receivedPacket._enemyPositionX != _enemy.transform.position.x)
        {
            // 동기화 되어있지 않다면 먼저 움직이는 모션을 넣어준다.
            var fixedPosition = _enemy.transform.position;
            fixedPosition.x = receivedPacket._enemyPositionX;
            fixedPosition.y = receivedPacket._enemyPositionY;
            _enemy.transform.position = fixedPosition;
        }

        _enemy.StartCoroutine("OnEnemyAttackStarted", receivedPacket);
    }

    // 게임이 끝났음을 알려주는 패킷 처리.
    private void OnGameSetNotify(GameSetNotify receivedPacket)
    {
        // 내가 이겼다면 승리 텍스트를, 아니라면 패배 텍스트를 내보낸다.
        if (receivedPacket._winPlayerNum == _dataContainer._playerNumber)
        {
            _endText.text = "PLAYER WIN!";
        }
        else
        {
            _endText.text = "PLAYER DEFEAT...";
        }

        StartCoroutine("OnGameSeted");

        // TODO :: 전적을 추가해준다.

    }

    // 턴이 자동 종료됨을 알려주는 패킷 처리.
    private void OnTurnAutoEnd()
    {
        // 내 턴일 때만 작동.
        if (_player._isMyTurn == false)
            return;

        // 우선 내 턴을 끝낸다.
        _player.TurnEndSetting();

        // 서버에게 내가 턴이 끝났다는 것을 알려준다.
        var turnEndNotify = new TurnEndNotify();
        _networkManager.SendPacket(turnEndNotify, PacketId.ID_TurnEndNotify);
    }

    // 매치 성사 응답을 보낸다.
    void SendMatchSuccessAck()
    {
        var successAck = new PacketInfo.MatchSuccessAck()
        {
            _result = (int)ErrorCode.None
        };

        _networkManager.SendPacket<PacketInfo.MatchSuccessAck>(successAck, PacketInfo.PacketId.ID_MatchSuccessAck);
    }

    // 캐릭터가 데미지를 입었을 경우 발생할 이벤트.
    void OnCharacterDamaged()
    {
        int player1Hp;
        int player2Hp;

        if (_dataContainer._playerNumber == 1)
        {
            player1Hp = _player._hp;
            player2Hp = _enemy._hp;
        }
        else
        {
            player1Hp = _enemy._hp;
            player2Hp = _player._hp;
        }

        var damageInfoPacket = new DamageOccur
        {
            _player1Hp = player1Hp,
            _player2Hp = player2Hp
        };

        _networkManager.SendPacket(damageInfoPacket, PacketId.ID_DamageOccur);
    }

    #endregion

    #region INGAME

    private Player _player;
    private Player _enemy;

    private IEnumerator PlayerCreate()
    {
        // 내 캐릭터 생성.
        var playerSpecText = Resources.Load<TextAsset>("Data/Archer" + (int)_dataContainer._playerType).text;
        var playerSpec = PlayerSpec.CreateFromText(playerSpecText);

        _player = Player.Factory.Create(playerSpec);
        _player.OnDamageOccured += OnCharacterDamaged;

        // 적 캐릭터 생성.
        var enemySpecText = Resources.Load<TextAsset>("Data/Archer" + (int)_dataContainer._enemyType).text;
        var enemySpec = PlayerSpec.CreateFromText(enemySpecText);

        _enemy = Player.Factory.Create(enemySpec);
        _enemy.OnDamageOccured += OnCharacterDamaged;
        _enemy.SetEnemy();

        // 이펙트 매니저에서 쓸 수 있도록 등록.
        _effectManager.SetPlayers(_player.gameObject, _enemy.gameObject);
        yield break;
    }

    private IEnumerator PlatformCreate()
    {
        var platform = Resources.Load<GameObject>("Prefabs/Platform");
        Instantiate(platform);
        yield break;
    }

    private IEnumerator OnTurnChanged(bool isEnemyTurnNow)
    {
        yield return new WaitForSeconds(3f);

        CameraFocusToPlayer(isEnemyTurnNow);

        string turnText;
        if (isEnemyTurnNow == true)
        {
            turnText = "ENEMY TURN";
        }
        else
        {
            turnText = "PLAYER TURN";
        }
        _turnText.text = turnText;

        _turnText.transform.DOMoveY(Screen.height * 0.6f, 2f);
        yield return new WaitForSeconds(2f);

        for (var i = 0; i < 2; ++i)
        {
            _turnText.text = "";
            yield return new WaitForSeconds(0.2f);
            _turnText.text = turnText;
            yield return new WaitForSeconds(0.2f);
        }

        _turnText.text = "";
        _turnText.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0f - 50f, 0);

        if (isEnemyTurnNow == false)
        {
            _player._isMyTurn = true;
        }
    }

    private IEnumerator OnGameSeted()
    {
        #region UI TURN OFF

        _timeText.enabled = false;

        _playerHealthBar.GetComponentInChildren<Image>().enabled = false;
        _playerHealthBar.GetComponentInChildren<Canvas>().enabled = false;
        _enemyHealthBar.GetComponentInChildren<Image>().enabled = false;
        _enemyHealthBar.GetComponentInChildren<Canvas>().enabled = false;

        _playerNameText.enabled = false;
        _enemyNameText.enabled = false;
        _playerText.enabled = false;
        _playerScoreText.enabled = false;
        _enemyText.enabled = false;
        _enemyScoreText.enabled = false;
        _turnText.enabled = false;

        #endregion

        _blackCurtain = Instantiate(Resources.Load("Prefabs/BlackCurtain") as GameObject).GetComponent<BlackCurtain>();
        _blackCurtain.Init();
        for (var i = 0f; i <= 0.5f; i += 0.01f)
        {
            Color color = new Color(1, 1, 1, i);
            _blackCurtain._spriteRenderer.color = color;
            yield return new WaitForSeconds(0.02f);
        }

        _endText.enabled = true;
        _endText.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);

        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene("3. Matching");
    }

    private void CameraFocusToPlayer(bool isEnemyTurnNow)
    {
        Player focusPlayer;

        if (isEnemyTurnNow == true)
        {
            focusPlayer = _enemy;
        }
        else
        {
            focusPlayer = _player;
        }

        var currentCameraPos = Camera.main.transform.position;
        var playerPos = focusPlayer.transform.position;
        playerPos.z = currentCameraPos.z;

        Camera.main.gameObject.Tween("CameraMove", currentCameraPos, playerPos, 1f, TweenScaleFunctions.CubicEaseIn, (t) =>
        {
            Camera.main.gameObject.transform.position = t.CurrentValue;
        });

        var currentCameraSize = Camera.main.orthographicSize;
        float afterSize = 10;

        Camera.main.gameObject.Tween("CameraZoomIn", currentCameraSize, afterSize, 1f, TweenScaleFunctions.CubicEaseIn, (t) =>
        {
            Camera.main.orthographicSize = t.CurrentValue;
        });
    }

    #endregion

    #region UI

    private UISystem _uiSystem;
    [SerializeField]
    Text _timeText;
    [SerializeField]
    GameObject _playerHealthBar;
    [SerializeField]
    GameObject _enemyHealthBar;
    [SerializeField]
    Text _playerNameText;
    [SerializeField]
    Text _enemyNameText;
    [SerializeField]
    Text _playerText;
    [SerializeField]
    Text _playerScoreText;
    [SerializeField]
    Text _enemyText;
    [SerializeField]
    Text _enemyScoreText;
    [SerializeField]
    Text _turnText;
    [SerializeField]
    Text _endText;
    [SerializeField]
    BlackCurtain _blackCurtain;

    private GameTimer _gameTimer;

    private void UIInitialize()
    {
        _uiSystem = FindObjectOfType<UISystem>();

        // 시간관련 초기화.
        _timeText = Instantiate(Resources.Load("GUI/TimeText") as GameObject).GetComponent<Text>();
        var timePosition = (new Vector3(Screen.width / 2, Screen.height * 0.95f, 0));
        timePosition.z = 0;
        _timeText.transform.position = timePosition;
        _uiSystem.AttachUI(_timeText.gameObject);

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
        _playerNameText = Instantiate(Resources.Load("GUI/PlayerNameText") as GameObject).GetComponent<Text>();
        _playerNameText.GetComponent<Text>().text = _dataContainer._playerId;
        _playerNameText.transform.position = new Vector3(Screen.width * 0.128f, Screen.height * 0.95f, 0);
        _uiSystem.AttachUI(_playerNameText.gameObject);

        _playerText = Instantiate(Resources.Load("GUI/PlayerText") as GameObject).GetComponent<Text>();
        _uiSystem.AttachUI(_playerText.gameObject);

        _playerScoreText = Instantiate(Resources.Load("GUI/PlayerScoreText") as GameObject).GetComponent<Text>();
        _playerScoreText.GetComponent<Text>().text = "Wins : " + _dataContainer._playerWins.ToString() + "\n Loses : " + _dataContainer._playerLoses.ToString();
        _playerScoreText.transform.position = new Vector3(Screen.width * 0.40f, Screen.height * 0.95f, 0);
        _uiSystem.AttachUI(_playerScoreText.gameObject);

        // 적군 정보 초기화.
        _enemyNameText = Instantiate(Resources.Load("GUI/EnemyNameText") as GameObject).GetComponent<Text>();
        _enemyNameText.GetComponent<Text>().text = _dataContainer._enemyId;
        _enemyNameText.transform.position = new Vector3(Screen.width * 0.872f, Screen.height * 0.95f, 0);
        _uiSystem.AttachUI(_enemyNameText.gameObject);

        _enemyText = Instantiate(Resources.Load("GUI/PlayerText") as GameObject).GetComponent<Text>();
        _uiSystem.AttachUI(_enemyText.gameObject);

        _enemyScoreText = Instantiate(Resources.Load("GUI/EnemyScoreText") as GameObject).GetComponent<Text>();
        _enemyScoreText.GetComponent<Text>().text = _dataContainer._enemyWins.ToString() + " : Wins \n" + _dataContainer._enemyLoses.ToString() + " : Loses";
        _enemyScoreText.transform.position = new Vector3(Screen.width * 0.60f, Screen.height * 0.95f, 0);
        _uiSystem.AttachUI(_enemyScoreText.gameObject);

        // 턴 텍스트 초기화.
        _turnText = Instantiate(Resources.Load("GUI/TurnText") as GameObject).GetComponent<Text>();
        _turnText.GetComponent<Text>().text = "PLAYER TURN";
        _turnText.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0f - 50f, 0);
        _uiSystem.AttachUI(_turnText.gameObject);

        // 게임 결과 텍스트 초기화.
        _endText = Instantiate(Resources.Load("GUI/EndText") as GameObject).GetComponent<Text>();
        _endText.enabled = false;
        _uiSystem.AttachUI(_endText.gameObject);

        // 트윈 초기화.
        DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
    }

    private void UIUpdate()
    {
        // 플레이어 정보가 따라다니도록.
        var playerScreenPosition = Camera.main.WorldToScreenPoint(_player.transform.position);
        playerScreenPosition.y += 80;
        _playerText.transform.position = playerScreenPosition;

        var enemyScreenPosition = Camera.main.WorldToScreenPoint(_enemy.transform.position);
        enemyScreenPosition.y += 80;
        _enemyText.transform.position = enemyScreenPosition;
    }

    #endregion
}
