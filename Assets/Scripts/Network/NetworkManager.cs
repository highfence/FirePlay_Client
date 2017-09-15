using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PacketInfo;

public partial class NetworkManager : MonoBehaviour 
{
    public HttpNetwork              _httpNetwork = null;
    public TcpNetwork               _tcpNetwork  = null;
    public Queue<PacketInfo.Packet> _packetQueue;

    #region SINGLETON

    private static NetworkManager _instance = null;

    private void Awake()
    {
        Initialize();
        DontDestroyOnLoad(this.gameObject);
    }

    public static NetworkManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = Instantiate(Resources.Load("Prefabs/NetworkManager") as GameObject).GetComponent<NetworkManager>();
        }
        return _instance;
    }

    public void Initialize()
    {
        // TcpNetwork 생성.
        _tcpNetwork = new TcpNetwork();
        _tcpNetwork.ConnectToServer();

        // HttpNetwork 생성
        _httpNetwork = Instantiate(Resources.Load("Prefabs/HttpNetwork") as GameObject).GetComponent<HttpNetwork>();
    }

    #endregion

    // 어플리케이션이 종료될 때 소켓을 닫아주는 메소드.
    void OnApplicationQuit()
    {
        // TODO :: Close Session Packet을 보내준다.
        _tcpNetwork.CloseNetwork();
    }

    private void Update()
    {
        if (_tcpNetwork.IsMessageExist())
        {
            var receivedPacket = _tcpNetwork.GetPacketFromQueue();

            InvokePacketEvents(receivedPacket);
        }
    }

    // 받은 패킷에 이벤트를 걸어놨던 함수들을 모두 실행시킨다.
    private void InvokePacketEvents(Packet receivedPacket)
    {
        switch ((PacketId)receivedPacket.packetId)
        {
            case PacketId.ID_LoginRes:
                this.OnLoginRes.Invoke(JsonUtility.FromJson<PacketInfo.LoginRes>(receivedPacket.data));
                break;
            case PacketId.ID_FastMatchRes:
                this.OnFastMatchRes.Invoke(JsonUtility.FromJson<PacketInfo.FastMatchRes>(receivedPacket.data));
                break;
            case PacketId.ID_MatchCancelRes:
                this.OnMatchCancelRes.Invoke(JsonUtility.FromJson<PacketInfo.MatchCancelRes>(receivedPacket.data));
                break;
            case PacketId.ID_MatchSuccessNotify:
                this.OnMatchSuccessNotify.Invoke(JsonUtility.FromJson<PacketInfo.MatchSuccessNotify>(receivedPacket.data));
                break;
            case PacketId.ID_GameStartNotify:
                this.OnGameStartNotify.Invoke(JsonUtility.FromJson<PacketInfo.GameStartNotify>(receivedPacket.data));
                break;
            case PacketId.ID_TurnStartNotify:
                this.OnTurnStartNotify.Invoke(JsonUtility.FromJson<PacketInfo.TurnStartNotify>(receivedPacket.data));
                break;
            case PacketId.ID_EnemyTurnStartNotify:
                this.OnEnemyTurnStartNotify.Invoke(JsonUtility.FromJson<PacketInfo.EnemyTurnStartNotify>(receivedPacket.data));
                break;
            case PacketId.ID_MoveAck:
                this.OnMoveAck.Invoke(JsonUtility.FromJson<PacketInfo.MoveAck>(receivedPacket.data));
                break;
            case PacketId.ID_EnemyMoveNotify:
                this.OnEnemyMoveNotify.Invoke(JsonUtility.FromJson<PacketInfo.EnemyMoveNotify>(receivedPacket.data));
                break;
            case PacketId.ID_FireAck:
                this.OnFireAck.Invoke(JsonUtility.FromJson<PacketInfo.FireAck>(receivedPacket.data));
                break;
            case PacketId.ID_EnemyFireNotify:
                this.OnEnemyFireNotify.Invoke(JsonUtility.FromJson<PacketInfo.EnemyFireNotify>(receivedPacket.data));
                break;
            case PacketId.ID_GameSetNotify:
                this.OnGameSetNotify.Invoke(JsonUtility.FromJson<PacketInfo.GameSetNotify>(receivedPacket.data));
                break;
        }
    }

    //----------------------------------- 아랫단부터는 없어져야할 코드.

    // 컴포넌트 HttpNetwork의 PostRequest 래핑 메소드.
    public void HttpPost<T>(string url, string bodyJson, Func<T, bool> onSuccess)
    {
        StartCoroutine(_httpNetwork.PostRequest<T>(url, bodyJson, onSuccess));
    }

    // 컴포넌트 TcpNetwork의 Send를 호출해주는 래핑 메소드.
    public void SendPacket<T>(T data, PacketInfo.PacketId packetId)
    {
        _tcpNetwork.SendPacket<T>(data, packetId);
    }
}
