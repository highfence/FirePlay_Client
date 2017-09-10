using System.Collections;
using System.Collections.Generic;
using PacketInfo;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public partial class PacketProcessor : MonoBehaviour
{
    // 패킷 처리 함수 형태 선언.
    delegate void PacketProcessFunction(string jsonData);
    // 패킷 ID에 따른 처리 함수를 저장할 딕셔너리
    Dictionary<int, PacketProcessFunction> _packetFunctionDic = new Dictionary<int, PacketProcessFunction>();

    // 입력 패킷을 받고 해당하는 ID의 메소드를 호출해주는 메소드.
    public bool Process(Packet packet)
    {
        Debug.LogFormat("Process Packet Id : {0}", packet.packetId);

        // 들어온 패킷에 해당하는 함수가 있다면 실행.
        if (_packetFunctionDic.ContainsKey(packet.packetId))
        {
            _packetFunctionDic[packet.packetId](packet.data);
        }
        else
        {
            Debug.LogErrorFormat("Invalid Packet Id : {0}", packet.packetId);
        }

        return true;
    }

    // 패킷 ID에 대응되는 함수들을 등록해주는 메소드.
    public void RegistPacketFunctions()
    {
        _packetFunctionDic.Add((int)PacketId.ID_CloseReq, ConnectCloseReq);
        _packetFunctionDic.Add((int)PacketId.ID_LoginRes, LoginRes);
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
            _token = (myInfo._token)
        };

        network._tcpNetwork.SendPacket<CloseReq>(req, PacketId.ID_CloseReq);
    }

    // 클라이언트 로그인 응답 메소드
    void LoginRes(string inData)
    {
        var res = new LoginRes();
        res = JsonUtility.FromJson<LoginRes>(inData);

        // 요청이 성공적이었다면, 씬을 바꿔준다.
        if (res._result == (int)ErrorCode.None)
        {
            SceneManager.LoadScene("CharacterSelect");
        }
        else
        {
            Debug.LogFormat("LoginRes Result : {0}", res._result);
        }
    }
}
