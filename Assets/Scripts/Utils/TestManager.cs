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

    private void Start()
    {
        PlatformCreate();
        PlayerCreate();

        EffectManager.GetInstance().SetPlayers(_player.gameObject, _enemy.gameObject);
        _dataContainer = DataContainer.GetInstance();
        UIInitialize();
    }

    private UISystem _uiSystem;
    public GameObject _timeText;
    public GameObject _playerHealthBar;
    public GameObject _enemyHealthBar;
    public GameObject _playerNameText;
    public GameObject _enemyNameText;

    private GameTimer _gameTimer;

    private void UIInitialize()
    {
        _uiSystem = FindObjectOfType<UISystem>();

        // 시간관련 초기화.
        _timeText = Instantiate(Resources.Load("GUI/TimeText")) as GameObject;
        var timePosition = _uiSystem._uiCam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height * 0.8f, 0));
        timePosition.z = 0;
        _timeText.transform.position = timePosition;
        _uiSystem.AttachUI(_timeText);

        _gameTimer = Instantiate(Resources.Load("Prefabs/GameTimer") as GameObject).GetComponent<GameTimer>();
        _gameTimer.SetText(_timeText);
        _gameTimer.OnTurnAutoEnd += OnTurnAutoEnd;

        // 플레이어 체력바 초기화.
        _playerHealthBar = Instantiate(Resources.Load("GUI/HorizontalBoxWithShadow") as GameObject);
        _playerHealthBar.transform.position = new Vector3(130, 357, 0);
        _uiSystem.AttachUI(_playerHealthBar);

        // 적군 체력바 초기화.
        _enemyHealthBar = Instantiate(Resources.Load("GUI/HorizontalBoxWithShadow") as GameObject);
        _enemyHealthBar.transform.position = new Vector3(657, 357, 0);
        _uiSystem.AttachUI(_enemyHealthBar);

        // 플레이어 정보 초기화.
        _playerNameText = Instantiate(Resources.Load("GUI/NameText")) as GameObject;
        _playerNameText.GetComponent<Text>().text = _dataContainer._playerId;

        // 적군 정보 초기화.
        _enemyNameText = Instantiate(Resources.Load("GUI/NameText")) as GameObject;
        _enemyNameText.GetComponent<Text>().text = _dataContainer._enemyId;
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
