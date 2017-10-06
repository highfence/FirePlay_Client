using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
    }

    #region PACKET LOGIC

    DataContainer _dataContainer = null;
    NetworkManager _networkManager = null;

    private void Initialize()
    {
        _dataContainer = DataContainer.GetInstance();
        _networkManager = NetworkManager.GetInstance();
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
        // TODO :: ScreenToWorldPoint로 바꿀듯.
        _player.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(receivedPacket._positionX, 30));
        _dataContainer.SetPlayerPosition(_player.transform.position);

        _enemy.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(receivedPacket._enemyPositionX, 30));
        _dataContainer.SetEnemyPosition(_enemy.transform.position);

        Debug.LogFormat("received point : {0}, {1}", receivedPacket._positionX, receivedPacket._enemyPositionX);

        // 응답을 보내준다.
        var ackPacket = new PacketInfo.GameStartAck()
        {
            _result = (int)ErrorCode.None
        };

        _networkManager.SendPacket<PacketInfo.GameStartAck>(ackPacket, PacketInfo.PacketId.ID_GameStartAck);
        Debug.Log("Send Game Start Ack");

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
        _enemy.transform.position = new Vector3(receivedPacket._enemyPositionX, receivedPacket._enemyPositionY);
        _dataContainer.SetEnemyPosition(_enemy.transform.position);

        //var movedRange = Camera.main.WorldToViewportPoint(new Vector3(receivedPacket._moveRange, 0, 0)).x;
        //MoveEnemy(receivedPacket._enemyPositionX, (int)movedRange);
    }

    //IEnumerator MoveEnemy(int enemyPositionX, int movedRange)
    //{
    //    float remainRange = (float)movedRange;
    //    if (remainRange < 0)
    //    {
    //        remainRange *= -1;
    //    }

    //    while (remainRange > 0.0f)
    //    {
    //        var enemyPos = _enemy.transform.position;
    //        enemyPos.x += movedRange / 100;
    //        _enemy.transform.position = enemyPos;
    //        remainRange -= movedRange / 100;

    //        yield return new WaitForSeconds(0.02f);
    //    }
    //}

    private void OnFireAck(PacketInfo.FireAck receivedPacket)
    {

    }

    private void OnEnemyFireNotify(PacketInfo.EnemyFireNotify receivedPacket)
    {

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

    private GameTimer _gameTimer;

    public void UIInitialize()
    {
        _uiSystem = FindObjectOfType<UISystem>();

        _gameTimer = Instantiate(Resources.Load("Prefabs/GameTimer") as GameObject).GetComponent<GameTimer>();
        _gameTimer.SetText(_timeText);
        _gameTimer.OnTurnAutoEnd += OnTurnAutoEnd;
        _gameTimer.TurnStart();
    }

    #endregion
}
