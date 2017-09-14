using System;

/*
 * TcpNetwork 이벤트들을 정리하는 파셜 클래스.
 * 해당 패킷을 전달 받았을 때 실행시키고 싶은 이벤트를 걸어주면 된다.
 */
public partial class TcpNetwork
{
    public event Action<PacketInfo.LoginRes>             OnLoginRes             = delegate { };
    public event Action<PacketInfo.FastMatchRes>         OnFastMatchRes         = delegate { };
    public event Action<PacketInfo.MatchCancelRes>       OnMatchCancelRes       = delegate { };
    public event Action<PacketInfo.MatchSuccessNotify>   OnMatchSuccessNotify   = delegate { };
    public event Action<PacketInfo.GameStartNotify>      OnGameStartNotify      = delegate { };
    public event Action<PacketInfo.TurnStartNotify>      OnTurnStartNotify      = delegate { };
    public event Action<PacketInfo.EnemyTurnStartNotify> OnEnemyTurnStartNotify = delegate { };
    public event Action<PacketInfo.MoveAck>              OnMoveAck              = delegate { };
    public event Action<PacketInfo.EnemyMoveNotify>      OnEnemyMoveNotify      = delegate { };
    public event Action<PacketInfo.FireAck>              OnFireAck              = delegate { };
    public event Action<PacketInfo.EnemyFireNotify>      OnEnemyFireNotify      = delegate { };
    public event Action<PacketInfo.GameSetNotify>        OnGameSetNotify        = delegate { };
}
