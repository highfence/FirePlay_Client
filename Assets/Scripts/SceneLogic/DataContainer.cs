using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 로직에 필요한 데이터들을 담아두는 클래스.
 * 씬 전환시에 관련 씬 매니저가 할당 해제되어도 기록되어야 할 필요가 있는 정보를 기록한다.
 * 싱글톤으로 접근.
 * 당연히 씬 전환에 사라지지 않는다.
 */
public class DataContainer : MonoBehaviour
{
    // 싱글톤 구현.
    private static DataContainer _instance = null;
    private static GameObject _container = null;

    public static DataContainer GetInstance()
    {
        if (_instance == null)
        {
            _container = new GameObject
            {
                name = "DataContainer"
            };
            _instance = _container.AddComponent(typeof(DataContainer)) as DataContainer;
            _instance.Initialize();
        }

        return _instance;
    }

    // 초기화 메소드.
    private void Initialize()
    {
        DontDestroyOnLoad(_container);
    }




}
