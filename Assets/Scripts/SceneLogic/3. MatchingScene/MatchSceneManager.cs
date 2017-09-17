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
        // 정상적인 답변이 왔다면 그냥 매칭 성사 패킷을 기다리면 된다.
        if (receivedPacket._result == (int)ErrorCode.None)
        {
            return;
        }

        // 정상적인 답변이 아니라면, 다시 한번 매칭 패킷을 보낸다.
        // TODO :: 이러면 쓸데 없는 패킷이 너무 많이 왔다갔다 하니까, 
        // 패킷에 마지막 보낸 시간을 적어놓고 1초에 한번씩 보내게 한다던가 하는 것이 필요함.
        // TODO :: 에러코드를 분석해서 그에 따른 switch ~ case 문으로 처리하는 것도 좋을듯.
        // 우선 지금은 바로 다시 요청 패킷을 보낸다.
        SendMatchReqPacket();
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

    }

    // 매치 취소 요청 패킷을 보낸다.
    void SendMatchCancelPacket()
    {
        var cancelReq = new PacketInfo.MatchCancelReq()
        {
            _id = _dataContainer._playerId,
            _token = _dataContainer._playerToken
        };

        _networkManager.SendPacket<PacketInfo.MatchCancelReq>(cancelReq, PacketInfo.PacketId.ID_MatchCancelReq);
    }
}
