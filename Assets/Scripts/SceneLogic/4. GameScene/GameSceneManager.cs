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
        _networkManager.OnGameStartNotify += this.OnGameStartNotify;
    }

    // 게임 시작 알림 답변 패킷 처리.
    private void OnGameStartNotify(PacketInfo.GameStartNotify receivedPacket)
    {
        // 받은 패킷에서 내 위치와 상대방 위치를 뽑아옴.
        _player.transform.position = Camera.main.WorldToViewportPoint(new Vector3(receivedPacket._positionX, 30));
        _enemy.transform.position = Camera.main.WorldToViewportPoint(new Vector3(receivedPacket._enemyPositionX, 30));

        Debug.LogFormat("received point : {0}, {1}, translated point : {2}, {3}", receivedPacket._positionX, receivedPacket._positionY, _player.transform.position.x, _player.transform.position.y);

        // 응답을 보내준다.
        var ackPacket = new PacketInfo.GameStartAck()
        {
            _result = (int)ErrorCode.None
        };

        _networkManager.SendPacket<PacketInfo.GameStartAck>(ackPacket, PacketInfo.PacketId.ID_GameStartAck);

        // TODO :: 컷씬이 추가된다면 컷씬 추가 
    }

    #endregion

    #region INGAME GRAPHIC

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
    }

    private void PlatformCreate()
    {
        var platform = Resources.Load<GameObject>("Prefabs/Platform");
        Instantiate(platform);
    }

    #endregion
}
