using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * 매칭 씬을 담당하는 클래스.
 */
public class MatchSceneManager : MonoBehaviour
{
    #region PACKET LOGIC

    DataContainer _dataContainer = null;
    NetworkManager _networkManager = null;

    private void Start()
    {
        Initialize();
        RegistPacketEvents();

        SetTextOption();
        MakeAnim();
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
        _networkManager.OnFastMatchRes       += OnFastMatchRes;
        _networkManager.OnMatchCancelRes     += OnMatchCancelRes;
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
        // TODO :: OnFastmatchRes와 같은 고려 필요. 
        if (receivedPacket._result == (int)ErrorCode.None)
        {
            return;
        }

        SendMatchCancelPacket();
    }

    // 매치 성공 알림 답변 패킷 처리.
    private void OnMatchSuccessNotify(PacketInfo.MatchSuccessNotify receivedPacket)
    {
        // 받은 정보를 저장한다.
        _dataContainer.SetGameMatchData(receivedPacket);

        // 게임 씬으로 상태를 바꾼다.
        SceneManager.LoadScene("4. Game");
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



    #endregion

    #region GUI

    private GameObject _curtain             = null;
    private GUIStyle   _textStyle           = null;
    private GameObject _modelInstance       = null;
    private bool       _isTryingNumberMatch = false;
    private bool       _isTryingFastMatch   = false;
    private float      _matchingCountTime   = 0.0f;
    private string[]   _matchingString;
    private int        _matchingCountIdx    = 0;

    private void OnGUI()
    {
        MakeButton();
    }

    private void MakeAnim()
    {
        var selectInfo = _dataContainer._playerType;

        string prefabPath = "PrivateData/SpritesArchers/FantasyArcher_0" + (int)selectInfo;

        _modelInstance = Instantiate(Resources.Load(prefabPath) as GameObject);

        _modelInstance.GetComponent<Transform>().position = new Vector3(0f, 1f, 0f);
    }

    private void MakeButton()
    {
        if (_isTryingFastMatch == false && _isTryingNumberMatch == false)
        {
            if (GUI.Button(new Rect((Screen.width / 2) - 155, (Screen.height * 2 / 3), 150, 150), "Fast Match"))
            {
                _isTryingFastMatch = true;

                SendMatchReqPacket();
                _curtain.GetComponent<BlackCurtain>().StartFadeIn();
            }
            if (GUI.Button(new Rect((Screen.width / 2) + 5, (Screen.height * 2 / 3), 150, 150), "Number Match"))
            {
                // TODO :: Number Match는 추후에 구현.
                _curtain.GetComponent<BlackCurtain>().StartFadeIn();

                _isTryingNumberMatch = true;
            }
        }
        else if (_isTryingNumberMatch == true)
        {
            TryNumberMatch();
        }
        else
        {
            TryFastMatch();
        }
    }

    // 텍스트 옵션을 세팅해주는 함수.
    private void SetTextOption()
    {
        _textStyle = new GUIStyle();
        _textStyle.fontSize = 40;
        _textStyle.normal.textColor = Color.white;

        _matchingString = new String[3];
        _matchingString[0] = "Matching.";
        _matchingString[1] = "Matching..";
        _matchingString[2] = "Matching...";
    }

    private void TryFastMatch()
    {
        MakeMatchWaitingScene();
    }

    private void TryNumberMatch()
    {
        MakeMatchWaitingScene();
    }

    private void MakeMatchWaitingScene()
    {
        _modelInstance.GetComponent<Animator>().enabled = false;
        _matchingCountTime += Time.deltaTime;

        if (_matchingCountTime >= 1.0f)
        {
            _matchingCountTime = 0f;
            ++_matchingCountIdx;
            if (_matchingCountIdx == 3) _matchingCountIdx = 0;
        }

        GUI.Label(new Rect((Screen.width / 2) - 100, Screen.height / 3, 200, 100), _matchingString[_matchingCountIdx], _textStyle);

        if (GUI.Button(new Rect((Screen.width / 2) - 100, Screen.height * 2 / 3, 200, 100), "Cancel"))
        {
            _curtain.GetComponent<BlackCurtain>().StartFadeOut();
            _isTryingNumberMatch = false;
            _isTryingFastMatch = false;
            _modelInstance.GetComponent<Animator>().enabled = true;
        }
    }

    #endregion
}
