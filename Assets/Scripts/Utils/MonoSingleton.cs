using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * MonoBehaviour면서 싱글톤인 클래스.
 */
public class MonoSingleton : MonoBehaviour
{
    public static MonoSingleton _instance = null;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
}
