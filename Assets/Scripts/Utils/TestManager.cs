using DG.Tweening;
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

        StartCoroutine("OnTurnChanged", false);
    }

    private void Update()
    {
        // 플레이어 정보가 따라다니도록.
        var playerScreenPosition = Camera.main.WorldToScreenPoint(_player.transform.position);
        playerScreenPosition.y += 80;
        _playerText.transform.position = playerScreenPosition;

        var enemyScreenPosition = Camera.main.WorldToScreenPoint(_enemy.transform.position);
        enemyScreenPosition.y += 80;
        _enemyText.transform.position = enemyScreenPosition;


        #region FOR TEST
        if (Input.GetKeyDown(KeyCode.S))
        {
            _enemy.StartCoroutine("OnMoveCommanded", _player.transform.position.x);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            var fireInfo = new PacketInfo.EnemyFireNotify()
            {
                _enemyPositionX = _enemy.transform.position.x,
                _fireType = 0,
                _magnitude = 30,
                _unitVecX = 1,
                _unitVecY = 1
            };

            _enemy.StartCoroutine("OnEnemyAttackStarted", fireInfo);
        }
        #endregion
    }

    private UISystem _uiSystem;
    public GameObject _playerHealthBar;
    public GameObject _enemyHealthBar;
    public Text _timeText;
    public Text _playerNameText;
    public Text _enemyNameText;
    public Text _playerText;
    public Text _playerScoreText;
    public Text _enemyText;
    public Text _enemyScoreText;
    public Text _turnText;


    private GameTimer _gameTimer;

    private IEnumerator OnTurnChanged(bool isEnemyTurnNow = false)
    {
        string turnText;
        if (isEnemyTurnNow == true)
        {
            turnText = "ENEMY TURN";
        }
        else
        {
            turnText = "PLAYER TURN";
        }
        _turnText.GetComponent<Text>().text = turnText;

        _turnText.transform.DOMoveY(Screen.height * 0.6f, 2f);
        yield return new WaitForSeconds(2f);

        for (var i = 0; i < 2; ++i)
        {
            _turnText.GetComponent<Text>().text = "";
            yield return new WaitForSeconds(0.2f);
            _turnText.GetComponent<Text>().text = turnText;
            yield return new WaitForSeconds(0.2f);
        }

        _turnText.GetComponent<Text>().text = "";
        _turnText.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0f - 50f, 0);

        if (isEnemyTurnNow == false)
        {
            _player._isMyTurn = true;
        }

    }

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

        // 트윈 초기화.
        DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
 
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
