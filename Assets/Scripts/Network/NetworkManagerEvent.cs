using System;

/*
 * NetworkManger의 이벤트들을 정리하는 파셜 클래스.
 */
public partial class NetworkManager : MonoSingleton
{
    public event Action<PacketInfo.LoginRes> OnLoginRes = delegate { };
}
