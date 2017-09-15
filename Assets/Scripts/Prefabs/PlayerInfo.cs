using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 씬이 바뀔 때 Player의 정보를 지니고 있을 구조체.
public class PlayerInfo : MonoBehaviour
{
	public void InfoSetting(string id, string token)
	{
		_id = id;
		_token = token;
	}

	public string _id;
	public string _token;
    public CharacterType _selectedPlayerType = CharacterType.None;
}