namespace PacketInfo
{
	// C++ IOCP서버와 Unity C#이 통신하기 위한 패킷 정의 파일입니다.
	// 로그인 요청 패킷
	public class LoginReq
{
	public string				_id;
	public string				_token;
	}

	// 로그인 답변 패킷
	public class LoginRes
{
	public int					_result;
	}

	// 빠른 매치 요청 패킷
	public class FastMatchReq
{
	public int					_type;
	public string				_id;
	public string				_token;
	}

	// 매치 요청 답변 패킷
	public class FastMatchRes
{
	public int					_result;
	}

	// 매치 취소 패킷
	public class MatchCancelReq
{
	public string				_id;
	public string				_token;
	}

	// 매치 취소 답변 패킷
	public class MatchCancelRes
{
	public int					_result;
	}

	// 매칭 성사 알림 패킷
	public class MatchSuccessNotify
{
	public int					_gameNumber;
	public string				_enemyId;
	public int					_enemyWins;
	public int					_enemyLoses;
	public int					_enemyType;
	}

	// 매칭 성사 응답 패킷
	public class MatchSuccessAck
{
	public int					_result;
	}

	// 게임 시작 알림 패킷
	public class GameStartNotify
{
	public int					_playerNumber;
	public int					_positionX;
	public int					_positionY;
	public int					_enemyPositionX;
	public int					_enemyPositionY;
	}

	// 게임 시작 응답 패킷
	public class GameStartAck
{
	public int					_result;
	}

	// 내 턴 시작 패킷
	public class TurnStartNotify
{
	public int					_windX;
	public int					_windY;
	}

	// 내 턴 시작 응답 패킷
	public class TurnStartAck
{
	public int					_result;
	}

	// 상대 턴 시작 패킷
	public class EnemyTurnStartNotify
{
	public int					_windX;
	public int					_windY;
	}

	// 상대 턴 시작 응답 패킷
	public class EnemyTurnStartAck
{
	public int					_result;
	}

	// 움직임 전달 패킷
	public class MoveNotify
{
	public int					_moveRange;
	public int					_enemyPositionX;
	public int					_enemyPositionY;
	}

	// 움직임 응답 패킷
	public class MoveAck
{
	public int					_result;
	}

	// 상대 움직임 알림 패킷
	public class EnemyMoveNotify
{
	public int					_moveRange;
	public int					_enemyPositionX;
	public int					_enemyPositionY;
	}

	// 상대 움직임 응답 패킷
	public class EnemyMoveAck
{
	public int					_result;
	}

	// 발사 패킷
	public class FireNotify
{
	public int					_fireType;
	public int					_enemyPositionX;
	public int					_enemyPositionY;
	public int					_forceX;
	public int					_forceY;
	}

	// 발사 응답 패킷
	public class FireAck
{
	public int					_result;
	}

	// 상대 발사 패킷
	public class EnemyFireNotify
{
	public int					_fireType;
	public int					_enemyPositionX;
	public int					_enemyPositionY;
	public int					_forceX;
	public int					_forceY;
	}

	// 상대 발사 응답 패킷
	public class EnemyFireAck
{
	public int					_result;
	}

	// 턴 종료 알림 패킷
	public class TurnEndNotify
{
	}

	// 턴 종료 응답 패킷
	public class TurnEndAck
{
	}

	// 게임 종료 알림 패킷
	public class GameSetNotify
{
	public int					_winPlayerNum;
	}

	// 게임 종료 응답 패킷
	public class GameSetAck
{
	public int					_result;
	}

	// 접속 닫기 패킷
	public class CloseReq
{
	public string				_id;
	public string				_token;
	}

	public enum PacketId
	{
		ID_LoginReq			= 101,
		ID_LoginRes			= 102,
		ID_FastMatchReq			= 103,
		ID_FastMatchRes			= 104,
		ID_MatchCancelReq			= 105,
		ID_MatchCancelRes			= 106,
		ID_MatchSuccessNotify			= 107,
		ID_MatchSuccessAck			= 108,
		ID_GameStartNotify			= 109,
		ID_GameStartAck			= 110,
		ID_TurnStartNotify			= 111,
		ID_TurnStartAck			= 112,
		ID_EnemyTurnStartNotify			= 113,
		ID_EnemyTurnStartAck			= 114,
		ID_MoveNotify			= 115,
		ID_MoveAck			= 116,
		ID_EnemyMoveNotify			= 117,
		ID_EnemyMoveAck			= 118,
		ID_FireNotify			= 119,
		ID_FireAck			= 120,
		ID_EnemyFireNotify			= 121,
		ID_EnemyFireAck			= 122,
		ID_TurnEndNotify			= 123,
		ID_TurnEndAck			= 124,
		ID_GameSetNotify			= 125,
		ID_GameSetAck			= 126,
		ID_CloseReq			= 127,
	};
}