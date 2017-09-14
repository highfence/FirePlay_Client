using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * 게임 시작전에 처리해야할 것들이 있다면 처리해주는 클래스.
 * 싱글톤 클래스 초기화를 위해 제작.
 */
public class LaunchManager : MonoBehaviour
{
	void Start ()
    {
        // 데이터 컨테이너 클래스 초기화.
        var dataObject = new DataContainer();
        var dataInstance = Instantiate<DataContainer>(dataObject);
        dataInstance.Initialize();

        // 네트워크 클래스 초기화.
        var networkObject = new NetworkManager();
        var networkInstance = Instantiate<NetworkManager>(networkObject);
        networkInstance.Initialize();

        SceneManager.LoadScene("Login");
	}
}
