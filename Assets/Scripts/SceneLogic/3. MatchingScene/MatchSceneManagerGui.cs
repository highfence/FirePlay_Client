using System;
using UnityEngine;

/*
 * 매칭 씬 Gui를 담당하는 클래스
 */
public partial class MatchSceneManager : MonoBehaviour
{
    private GameObject _curtain             = null;
    private GUIStyle   _textStyle           = null;
    private GameObject _modelInstance       = null;
    private bool       _isTryingNumberMatch = false;
    private bool       _isTryingFastMatch   = false;
    private float      _matchingCountTime   = 0.0f;
    private string[]   _matchingString;
    private int        _matchingCountIdx    = 0;

    private void Awake()
    {
        SetTextOption();
        MakeAnim();
    }

    private void OnGUI()
    {
        MakeButton();
    }

    private void MakeAnim()
    {
        var selectInfo = FindObjectOfType<PlayerInfo>();

        if (selectInfo == null) return;

        string prefabPath = "PrivateData/SpritesArchers/FantasyArcher_0" + (int)selectInfo._selectedPlayerType;

        _modelInstance = Instantiate(Resources.Load(prefabPath) as GameObject);

        _modelInstance.GetComponent<Transform>().position = new Vector3(0f, 1f, 0f);
    }

    private void MakeButton()
    {
        if (_isTryingFastMatch == false && _isTryingNumberMatch == false)
        {
            if (GUI.Button(new Rect((Screen.width / 2) - 155, (Screen.height * 2 / 3), 150, 150), "Fast Match"))
            {
                _isTryingFastMatch = true;

                SendMatchReqPacket();
                _curtain.GetComponent<BlackCurtain>().StartFadeIn();
            }
            if (GUI.Button(new Rect((Screen.width / 2) + 5, (Screen.height * 2 / 3), 150, 150), "Number Match"))
            {
                // TODO :: Number Match는 추후에 구현.
                _curtain.GetComponent<BlackCurtain>().StartFadeIn();

                _isTryingNumberMatch = true;
            }
        }
        else if (_isTryingNumberMatch == true)
        {
            TryNumberMatch();
        }
        else
        {
            TryFastMatch();
        }
    }

    // 텍스트 옵션을 세팅해주는 함수.
    private void SetTextOption()
    {
        _textStyle = new GUIStyle();
        _textStyle.fontSize = 40;
        _textStyle.normal.textColor = Color.white;

        _matchingString = new String[3];
        _matchingString[0] = "Matching.";
        _matchingString[1] = "Matching..";
        _matchingString[2] = "Matching...";
    }

    private void TryFastMatch()
    {
        MakeMatchWaitingScene();
    }

    private void TryNumberMatch()
    {
        MakeMatchWaitingScene();
    }

    private void MakeMatchWaitingScene()
    {
        _modelInstance.GetComponent<Animator>().enabled = false;
        _matchingCountTime += Time.deltaTime;

        if (_matchingCountTime >= 1.0f)
        {
            _matchingCountTime = 0f;
            ++_matchingCountIdx;
            if (_matchingCountIdx == 3) _matchingCountIdx = 0;
        }

        GUI.Label(new Rect((Screen.width / 2) - 100, Screen.height / 3, 200, 100), _matchingString[_matchingCountIdx], _textStyle);

        if (GUI.Button(new Rect((Screen.width / 2) - 100, Screen.height * 2 / 3, 200, 100), "Cancel"))
        {
            _curtain.GetComponent<BlackCurtain>().StartFadeOut();
            _isTryingNumberMatch = false;
            _isTryingFastMatch = false;
            _modelInstance.GetComponent<Animator>().enabled = true;
        }
    }
}
