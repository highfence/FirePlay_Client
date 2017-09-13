using UnityEngine;

/*
 * 로그인 씬 관리자 클래스.
 */
public partial class LoginSceneManager : MonoBehaviour
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

    // 로그인 서버에서 로그인 응답 패킷이 도착했을 경우 처리하는 콜백함수.
    private bool OnHttpLoginRes(HttpPack.LoginRes response)
    {
        var network = FindObjectOfType<NetworkManager>();

        switch ((ErrorCode)response.Result)
        {
            // 정상적으로 처리 된 경우.
            case ErrorCode.None:
                #region Error Code : None case
                if (string.IsNullOrEmpty(response.Token))
                {
                    return false;
                }

                // 유저 정보에 받은 토큰 내용 기록.
                _dataContainer.SetPlayerToken(response.Token);
                #endregion
                break;

            // 아이디나 비밀번호가 잘못된 경우.
            case ErrorCode.LoginUserInfoDontExist:
                #region Error Code : User Info Dont Exist 
                // TODO :: 회원가입중이라고 윈도우 띄우기.
                Debug.LogAssertion("Invalid Id or Pw");
                // 가입 요청을 한다.

                var signInReq = new HttpPack.LoginReq()
                {
                    UserId = _id,
                    UserPw = _pw
                };

                var jsonBody = JsonUtility.ToJson(signInReq);

                //var signInRequestUrl = 

                #endregion
                break;

        }

        return true;
    }
}
