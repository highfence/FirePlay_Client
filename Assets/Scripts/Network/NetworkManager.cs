using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public HttpNetwork     _httpNetwork = null;
    public TcpNetwork      _tcpNetwork  = null;
    public PacketProcessor _packetProcessor;

    private void Start()
    {
        // 씬 전환에도 소멸되지 않음.
        DontDestroyOnLoad(gameObject);

        // TcpNetwork 생성.
        _tcpNetwork = new TcpNetwork();
        _tcpNetwork.ConnectToServer();

        // HttpNetwork 생성
        _httpNetwork = new HttpNetwork();

        // 패킷 프로세서 생성.
        _packetProcessor = new PacketProcessor();
        _packetProcessor.RegistPacketFunctions();

        // 패킷을 처리할 수 있도록 메소드 연결.
        _tcpNetwork.RegistProcessFunc(_packetProcessor.Process);
    }

    private void Update()
    {
        // 처리할 패킷이 있다면 모두 다 처리.
        //while (_tcpNetwork.IsQueueEmpty() == false)
        //{
        //    var packet = _tcpNetwork.GetPacket();

        //    _packetProcessor.Process(packet);
        //}
    }

    // 컴포넌트 HttpNetwork의 PostRequest 래핑 메소드.
    public void HttpPost<T>(string url, string bodyJson, Func<T, bool> onSuccess)
    {
        StartCoroutine(_httpNetwork.PostRequest<T>(url, bodyJson, onSuccess));
    }

    // 컴포넌트 HttpNetwork의 Api를 찾아주는 래핑 메소드.
    public string GetApiString(LoginApiString apiEnum)
    {
        return _httpNetwork.GetApiString(apiEnum);
    }

    // 컴포넌트 TcpNetwork의 Send를 호출해주는 래핑 메소드.
    public void SendPacket<T>(T data, PacketInfo.PacketId packetId)
    {
        _tcpNetwork.SendPacket<T>(data, packetId);
    }

    // 어플리케이션이 종료될 때 소켓을 닫아주는 메소드.
    void OnApplicationQuit()
    {
        // TODO :: Close Session Packet을 보내준다.
        _tcpNetwork.CloseNetwork();
    }
}
