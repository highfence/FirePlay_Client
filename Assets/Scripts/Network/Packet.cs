namespace PacketInfo
{
	// C++ IOCP������ Unity C#�� ����ϱ� ���� ��Ŷ ���� �����Դϴ�.
	// �α��� ��û ��Ŷ
	public class LoginReq
{
	public string				_id;
	public string				_token;
	}

	// �α��� �亯 ��Ŷ
	public class LoginRes
{
	public int					_result;
	}

	// ���� ��ġ ��û ��Ŷ
	public class FastMatchReq
{
	public int					_type;
	public string				_id;
	public string				_token;
	}

	// ��ġ ��û �亯 ��Ŷ
	public class FastMatchRes
{
	public int					_result;
	}

	// ��ġ ��� ��Ŷ
	public class MatchCancelReq
{
	public string				_id;
	public string				_token;
	}

	// ��ġ ��� �亯 ��Ŷ
	public class MatchCancelRes
{
	public int					_result;
	}

	// ��Ī ���� �˸� ��Ŷ
	public class MatchSuccessNotify
{
	public int					_gameNumber;
	public string				_enemyId;
	public int					_enemyWins;
	public int					_enemyLoses;
	public int					_enemyType;
	}

	// ��Ī ���� ���� ��Ŷ
	public class MatchSuccessAck
{
	public int					_result;
	}

	// ���� ���� �˸� ��Ŷ
	public class GameStartNotify
{
	public int					_playerNumber;
	public int					_positionX;
	public int					_positionY;
	public int					_enemyPositionX;
	public int					_enemyPositionY;
	}

	// ���� ���� ���� ��Ŷ
	public class GameStartAck
{
	public int					_result;
	}

	// �� �� ���� ��Ŷ
	public class TurnStartNotify
{
	public int					_windX;
	public int					_windY;
	}

	// �� �� ���� ���� ��Ŷ
	public class TurnStartAck
{
	public int					_result;
	}

	// ��� �� ���� ��Ŷ
	public class EnemyTurnStartNotify
{
	public int					_windX;
	public int					_windY;
	}

	// ��� �� ���� ���� ��Ŷ
	public class EnemyTurnStartAck
{
	public int					_result;
	}

	// ������ ���� ��Ŷ
	public class MoveNotify
{
	public int					_moveRange;
	public int					_enemyPositionX;
	public int					_enemyPositionY;
	}

	// ������ ���� ��Ŷ
	public class MoveAck
{
	public int					_result;
	}

	// ��� ������ �˸� ��Ŷ
	public class EnemyMoveNotify
{
	public int					_moveRange;
	public int					_enemyPositionX;
	public int					_enemyPositionY;
	}

	// ��� ������ ���� ��Ŷ
	public class EnemyMoveAck
{
	public int					_result;
	}

	// �߻� ��Ŷ
	public class FireNotify
{
	public int					_fireType;
	public int					_enemyPositionX;
	public int					_enemyPositionY;
	public int					_forceX;
	public int					_forceY;
	}

	// �߻� ���� ��Ŷ
	public class FireAck
{
	public int					_result;
	}

	// ��� �߻� ��Ŷ
	public class EnemyFireNotify
{
	public int					_fireType;
	public int					_enemyPositionX;
	public int					_enemyPositionY;
	public int					_forceX;
	public int					_forceY;
	}

	// ��� �߻� ���� ��Ŷ
	public class EnemyFireAck
{
	public int					_result;
	}

	// �� ���� �˸� ��Ŷ
	public class TurnEndNotify
{
	}

	// �� ���� ���� ��Ŷ
	public class TurnEndAck
{
	}

	// ���� ���� �˸� ��Ŷ
	public class GameSetNotify
{
	public int					_winPlayerNum;
	}

	// ���� ���� ���� ��Ŷ
	public class GameSetAck
{
	public int					_result;
	}

	// ���� �ݱ� ��Ŷ
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