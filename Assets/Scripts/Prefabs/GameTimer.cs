﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public int _turnTime { get; private set; }
    public float _accTime { get; private set; }
    public bool _isTimeSeted { get; private set; }
    public bool _isTurnStarted { get; private set; }

    // 턴이 자동으로 끝났을 때 호출될 이벤트.
    public event Action OnTurnAutoEnd = delegate { };

    private GameObject _timeText;

	void Start ()
    {
        _turnTime = 15;
        _accTime = 0;
        _isTimeSeted = false;
	}

    public void SetText(GameObject textUI)
    {
        _timeText = textUI;
        _isTimeSeted = true;
    }
	
	void Update ()
    {
        if (_isTimeSeted && _isTurnStarted)
        {
            TimeProcess();
        }
	}

    public void TurnStart()
    {

    }

    private void TimeProcess()
    {
        _accTime += Time.deltaTime;

        while (_accTime >= 1.0f)
        {
            _accTime -= 1.0f;
            --_turnTime;

            if (_turnTime >= 0)
            {
                ApplyTextTime();
            }
            else
            {
                WaitForAutoTurnEnd();
            }
        }
    }

    private void ApplyTextTime()
    {
        if (_turnTime >= 10)
        {
            _timeText.GetComponent<Text>().text = _turnTime.ToString();
        }
        else
        {
            _timeText.GetComponent<Text>().text = "0" + _turnTime.ToString();
        }
    }

    private void WaitForAutoTurnEnd()
    {
        // 3초를 더 기다려준다.
        if (_turnTime <= -3)
        {
            // 턴 자동 종료 이벤트를 호출한다.
            OnTurnAutoEnd.Invoke(); 
        }
    }
}
