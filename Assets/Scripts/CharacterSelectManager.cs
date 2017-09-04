using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerType : int
{
	None = 0,
	Archer1 = 1,
	Archer2 = 2,
	Archer3 = 3
}

public class CharacterSelectManager : MonoBehaviour
{
	public PlayerType _selectedCharacter = PlayerType.Archer1;

	private GameObject   _pointer        = null;
    private GameObject[] _archers        = new GameObject[(int)PlayerType.Archer3 + 1];
    private Vector3[]    _archerPosition = new Vector3[(int)PlayerType.Archer3 + 1];
    private Vector3      _pointerVector  = new Vector3(0f, -3f, 0f);

	private void Start()
	{
		SetInitialArchers();
		SetInitialPointer();
	}

	private void Update()
	{
        ProcessKeyboardInput();
	}

	private void SetInitialArchers()
	{
		_archers[(int)PlayerType.Archer1] = Instantiate(Resources.Load("PrivateData/SpritesArchers/FantasyArcher_01") as GameObject);
        _archerPosition[(int)PlayerType.Archer1] = new Vector3(-4f, 0f, 0f);
        _archers[(int)PlayerType.Archer1].GetComponent<Transform>().position = _archerPosition[(int)PlayerType.Archer1];
        _archers[(int)PlayerType.Archer1].GetComponent<Animator>().enabled = false;

        _archers[(int)PlayerType.Archer2] = Instantiate(Resources.Load("PrivateData/SpritesArchers/FantasyArcher_02") as GameObject);
        _archerPosition[(int)PlayerType.Archer2] = new Vector3(0f, 0f, 0f);
        _archers[(int)PlayerType.Archer2].GetComponent<Transform>().position = _archerPosition[(int)PlayerType.Archer2];
        _archers[(int)PlayerType.Archer2].GetComponent<Animator>().enabled = false;

        _archers[(int)PlayerType.Archer3] = Instantiate(Resources.Load("PrivateData/SpritesArchers/FantasyArcher_03") as GameObject);
        _archerPosition[(int)PlayerType.Archer3] = new Vector3(4f, 0f, 0f);
        _archers[(int)PlayerType.Archer3].GetComponent<Transform>().position = _archerPosition[(int)PlayerType.Archer3];
        _archers[(int)PlayerType.Archer3].GetComponent<Animator>().enabled = false;
	}

	private void SetInitialPointer()
	{
		_pointer = Instantiate(Resources.Load("PrivateData/SelectPointer") as GameObject);
        // 시작 포인트는 Archer1에서 시작.
        _pointer.GetComponent<Transform>().position = _archerPosition[(int)PlayerType.Archer1] + _pointerVector;
        _archers[(int)PlayerType.Archer1].GetComponent<Animator>().enabled = true;
	}

    private void ProcessKeyboardInput()
    {
		GetPointerMove();

        GetSelection();
    }

	private void GetPointerMove()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
            // 가장 왼쪽일 때 왼쪽으로 가면 바로 리턴.
            if (_selectedCharacter == PlayerType.Archer1) return;

            // 원래 포인터가 있던 자리의 애니메이션을 멈춘다.
            _archers[(int)_selectedCharacter].SetActive(false);
            _archers[(int)_selectedCharacter].SetActive(true);
            _archers[(int)_selectedCharacter].GetComponent<Animator>().enabled = false;

            // 포인터를 이동시킨다.
            --_selectedCharacter;
            _pointer.GetComponent<Transform>().position = _archerPosition[(int)_selectedCharacter] + _pointerVector; 

            // 새로운 애니메이션을 활성화 시킨다.
            _archers[(int)_selectedCharacter].GetComponent<Animator>().enabled = true;
		}
        // 왼쪽일 때와 마찬가지.
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
            if (_selectedCharacter == PlayerType.Archer3) return;

            _archers[(int)_selectedCharacter].SetActive(false);
            _archers[(int)_selectedCharacter].SetActive(true);
            _archers[(int)_selectedCharacter].GetComponent<Animator>().enabled = false;

            ++_selectedCharacter;
            _pointer.GetComponent<Transform>().position = _archerPosition[(int)_selectedCharacter] + _pointerVector; 

            _archers[(int)_selectedCharacter].GetComponent<Animator>().enabled = true;
		}
	}

    private void GetSelection()
    {
        // 이상한 값일 경우 에러처리.
        if (_selectedCharacter <= PlayerType.None || _selectedCharacter > PlayerType.Archer3) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // PlayerInfo에 기록.
            var playerInfo = FindObjectOfType<PlayerInfo>();
            playerInfo._selectedPlayerType = _selectedCharacter;

            // 씬 전환.
            SceneManager.LoadScene("Matching");
        }
    }
}
