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
    }


}
