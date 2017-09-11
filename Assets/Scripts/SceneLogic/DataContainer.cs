using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 로직에 필요한 데이터들을 담아두는 클래스.
 * 씬 전환시에 관련 씬 매니저가 할당 해제되어도 기록되어야 할 필요가 있는 정보를 기록한다.
 * 싱글톤으로 접근.
 * 당연히 씬 전환에 사라지지 않는다.
 */
public class DataContainer : MonoSingleton 
{
    // 초기화 메소드.
    // 보유 자료중에 초기화가 필요한 자료가 있다면 여기서 처리.
    public void Initialize()
    {
        _playerInfo = new PlayerInfo();
    }

    // 플레이어의 정보를 담고 있는 구조체.
    public PlayerInfo _playerInfo { get; private set; }

}
