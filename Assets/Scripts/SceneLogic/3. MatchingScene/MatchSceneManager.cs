using System;
using UnityEngine;

/*
 * 매칭 씬을 담당하는 클래스.
 */
public partial class MatchSceneManager : MonoBehaviour
{
    DataContainer  _dataContainer  = null;
    NetworkManager _networkManager = null;

    private void Start()
    {
        Initialize();
        RegistPacketEvents();
    }

    // 초기화 함수.
    private void Initialize()
    {
        _dataContainer = DataContainer.GetInstance();
        _networkManager = NetworkManager.GetInstance();

        _curtain = Instantiate(Resources.Load("Prefabs/BlackCurtain") as GameObject);
    }

    // 패킷 처리 함수를 이벤트에 등록해주는 메소드.
    private void RegistPacketEvents()
    {
        _networkManager.OnFastMatchRes += OnFastMatchRes;
        _networkManager.OnMatchCancelRes += OnMatchCancelRes;
        _networkManager.OnMatchSuccessNotify += OnMatchSuccessNotify;
    }

    // 빠른 매치 요청 답변 패킷 처리.
    private void OnFastMatchRes(PacketInfo.FastMatchRes receivedPacket)
    {

    }

    // 매치 취소 요청 답변 패킷 처리.
    private void OnMatchCancelRes(PacketInfo.MatchCancelRes receivedPacket)
    {

    }

    // 매치 성공 알림 답변 패킷 처리.
    private void OnMatchSuccessNotify(PacketInfo.MatchSuccessNotify receivedPacket)
    {

    }

    // 매치 요청 패킷을 보낸다.
    void SendMatchReqPacket()
    {
        var matchReq = new PacketInfo.FastMatchReq()
        {
            _id = _dataContainer._playerId,
            _token = _dataContainer._playerToken,
            _type = (int)_dataContainer._playerType
        };

        _networkManager.SendPacket<PacketInfo.FastMatchReq>(matchReq, PacketInfo.PacketId.ID_FastMatchReq);

        _curtain.GetComponent<BlackCurtain>().StartFadeIn();
    }
}
