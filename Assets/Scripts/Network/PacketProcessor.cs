using System.Collections;
using System.Collections.Generic;
using PacketInfo;
using UnityEngine;
using System;

public partial class PacketProcessor : MonoBehaviour
{
    // 패킷 처리 함수 형태 선언.
    delegate void PacketProcessFunction(string jsonData);
    // 패킷 ID에 따른 처리 함수를 저장할 딕셔너리
    Dictionary<int, PacketProcessFunction> _packetFunctionDic;

	void Start ()
    {
        RegistPacketFunctions();
	}
	
    // 입력 패킷을 받고 해당하는 ID의 메소드를 호출해주는 메소드.
    public void Process(Packet packet)
    {
        // 들어온 패킷에 해당하는 함수가 있다면 실행.
        if (_packetFunctionDic.ContainsKey(packet.packetId))
        {
            _packetFunctionDic[packet.packetId](packet.data);
        }
    }

    // 패킷 ID에 대응되는 함수들을 등록해주는 메소드.
    void RegistPacketFunctions()
    {
        _packetFunctionDic = new Dictionary<int, PacketProcessFunction>();

        _packetFunctionDic.Add((int)PacketId.ID_CloseReq, ConnectCloseReq);
    }

    // 클라이언트 종료 요청 메소드.
    void ConnectCloseReq(string inData)
    {
        // 로직을 정리한다.

        // 종료 패킷을 보내달라고 요청한다.
        var network = FindObjectOfType<NetworkManager>();

        var myInfo = FindObjectOfType<PlayerInfo>();
        var req = new CloseReq()
        {
            _id    = myInfo._id,
            _token = Convert.ToInt32(myInfo._token)
        };

        network._tcpNetwork.SendPacket<CloseReq>(req, PacketId.ID_CloseReq);
    }
}
