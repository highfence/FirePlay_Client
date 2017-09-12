using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class LoginSceneManager : MonoBehaviour
{
    DataContainer _dataContainer;

    public void Awake()
    {
        _dataContainer = DataContainer._instance as DataContainer;
        var networkManager = NetworkManager._instance as NetworkManager;

        SubscribePacketEvents(networkManager);
    }

    // 패킷 도착 이벤트 메소드들을 처음에 등록해주는 함수.
    private void SubscribePacketEvents(NetworkManager network)
    {
        network.OnLoginRes += this.OnLoginResArrived;
    }

    // 게임 서버에서 로그인 응답 패킷이 도착했을 경우
    private void OnLoginResArrived(PacketInfo.LoginRes receivedPacket)
    {

    }

    private bool OnHttpLoginRes(HttpPack.LoginRes response)
    {
        var network = FindObjectOfType<NetworkManager>();


        return true;
    }
}
