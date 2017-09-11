using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Game 전체의 로직을 관리해주는 클래스.
 */
public class GameManager : MonoBehaviour
{
    private DataContainer _dataContainer = null;
    private NetworkManager _networkManager = null;
    private LoginSceneManager _loginSceneManager = null;

    private void Start()
    {
        // 씬 전환시 소멸되지 않음.
        DontDestroyOnLoad(gameObject);

        // 소유 클래스들 초기화.
        _dataContainer = (DataContainer)DataContainer._instance;
    }
}
