using UnityEngine;

/*
 * Character Select Scene 관리자 클래스.
 */
public partial class CharacterSelectSceneManager : MonoBehaviour
{
    DataContainer _dataContainer;
    NetworkManager _networkManager;

    public void Awake()
    {
        _dataContainer = DataContainer.GetInstance() as DataContainer;
        _networkManager = NetworkManager.GetInstance() as NetworkManager;

        SubscribePacketEvents(_networkManager);
    }

    // 패킷 도착 이벤트 메소드들을 처음에 등록해주는 함수.
    private void SubscribePacketEvents(NetworkManager network)
    {

    }

}
