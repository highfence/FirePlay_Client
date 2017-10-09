using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Character Select Scene 관리자 클래스.
 * GUI와 따로 나누어 놓지 않았다.
 */
public partial class CharacterSelectSceneManager : MonoBehaviour
{
    public GameObject _atkBar;
    public GameObject _defBar;
    public GameObject _spdBar;

    DataContainer _dataContainer;
    NetworkManager _networkManager;

    public void Awake()
    {
        _dataContainer = DataContainer.GetInstance() as DataContainer;
        _networkManager = NetworkManager.GetInstance() as NetworkManager;
    }

    public CharacterType _selectedCharacter = CharacterType.Archer1;

    private GameObject _pointer = null;
    private GameObject[] _archers = new GameObject[(int)CharacterType.Archer3 + 1];
    private Vector3[] _archerPosition = new Vector3[(int)CharacterType.Archer3 + 1];
    private Vector3 _pointerVector = new Vector3(0f, -2.2f, 0f);

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
        _archers[(int)CharacterType.Archer1] = Instantiate(Resources.Load("PrivateData/SpritesArchers/FantasyArcher_01") as GameObject);
        _archerPosition[(int)CharacterType.Archer1] = new Vector3(-4f, 1.5f, 0f);
        _archers[(int)CharacterType.Archer1].GetComponent<Transform>().position = _archerPosition[(int)CharacterType.Archer1];
        _archers[(int)CharacterType.Archer1].GetComponent<Transform>().localScale = new Vector3(0.7f, 0.7f);
        _archers[(int)CharacterType.Archer1].GetComponent<Animator>().enabled = false;

        _archers[(int)CharacterType.Archer2] = Instantiate(Resources.Load("PrivateData/SpritesArchers/FantasyArcher_02") as GameObject);
        _archerPosition[(int)CharacterType.Archer2] = new Vector3(0f, 1.5f, 0f);
        _archers[(int)CharacterType.Archer2].GetComponent<Transform>().position = _archerPosition[(int)CharacterType.Archer2];
        _archers[(int)CharacterType.Archer2].GetComponent<Transform>().localScale = new Vector3(0.7f, 0.7f);
        _archers[(int)CharacterType.Archer2].GetComponent<Animator>().enabled = false;

        _archers[(int)CharacterType.Archer3] = Instantiate(Resources.Load("PrivateData/SpritesArchers/FantasyArcher_03") as GameObject);
        _archerPosition[(int)CharacterType.Archer3] = new Vector3(4f, 1.5f, 0f);
        _archers[(int)CharacterType.Archer3].GetComponent<Transform>().position = _archerPosition[(int)CharacterType.Archer3];
        _archers[(int)CharacterType.Archer3].GetComponent<Transform>().localScale = new Vector3(0.7f, 0.7f);
        _archers[(int)CharacterType.Archer3].GetComponent<Animator>().enabled = false;
    }

    private void SetInitialPointer()
    {
        _pointer = Instantiate(Resources.Load("PrivateData/SelectPointer") as GameObject);
        // 시작 포인트는 Archer1에서 시작.
        _pointer.GetComponent<Transform>().position = _archerPosition[(int)CharacterType.Archer1] + _pointerVector;
        _archers[(int)CharacterType.Archer1].GetComponent<Animator>().enabled = true;
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
            if (_selectedCharacter == CharacterType.Archer1) return;

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
            if (_selectedCharacter == CharacterType.Archer3) return;

            _archers[(int)_selectedCharacter].SetActive(false);
            _archers[(int)_selectedCharacter].SetActive(true);
            _archers[(int)_selectedCharacter].GetComponent<Animator>().enabled = false;

            ++_selectedCharacter;
            _pointer.GetComponent<Transform>().position = _archerPosition[(int)_selectedCharacter] + _pointerVector;

            _archers[(int)_selectedCharacter].GetComponent<Animator>().enabled = true;
        }

        SetProgressBar();
    }

    private void SetProgressBar()
    {
        if (_selectedCharacter == CharacterType.Archer1)
        {
            _atkBar.GetComponent<ProgressBarPro>().Value = 0.66f;
            _defBar.GetComponent<ProgressBarPro>().Value = 0.35f;
            _spdBar.GetComponent<ProgressBarPro>().Value = 1;
        }
        else if (_selectedCharacter == CharacterType.Archer2)
        {
            _atkBar.GetComponent<ProgressBarPro>().Value = 0.75f;
            _defBar.GetComponent<ProgressBarPro>().Value = 0.75f;
            _spdBar.GetComponent<ProgressBarPro>().Value = 0.5f;
        }
        else
        {
            _atkBar.GetComponent<ProgressBarPro>().Value = 0.8f;
            _defBar.GetComponent<ProgressBarPro>().Value = 1;
            _spdBar.GetComponent<ProgressBarPro>().Value = 0.2f;

        }
    }

    private void GetSelection()
    {
        // 이상한 값일 경우 에러처리.
        if (_selectedCharacter <= CharacterType.None || _selectedCharacter > CharacterType.Archer3) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // PlayerInfo에 기록.
            _dataContainer.SetCharacterType(_selectedCharacter);

            // 씬 전환.
            SceneManager.LoadScene("3. Matching");
        }
    }
}
