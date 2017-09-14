﻿using System;
using UnityEngine;

public class NetworkManager : MonoBehaviour 
{
    public HttpNetwork     _httpNetwork = null;
    public TcpNetwork      _tcpNetwork  = null;

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
        _httpNetwork = new HttpNetwork();
    }

    #endregion


    // 어플리케이션이 종료될 때 소켓을 닫아주는 메소드.
    void OnApplicationQuit()
    {
        // TODO :: Close Session Packet을 보내준다.
        _tcpNetwork.CloseNetwork();
    }
    //----------------------------------- 아랫단부터는 없어져야할 코드.

    public PacketProcessor     _packetProcessor;

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
