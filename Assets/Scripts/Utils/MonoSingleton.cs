using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * MonoBehaviour면서 싱글톤인 클래스.
 * Launcher Scene에서 생성해주어야 한다.
 */
public class MonoSingleton : MonoBehaviour
{
    private static MonoSingleton _instance = null;

    public static MonoSingleton GetInstance() { return _instance; }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        else if (_instance != this)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    public virtual void Initialize() { }
}
