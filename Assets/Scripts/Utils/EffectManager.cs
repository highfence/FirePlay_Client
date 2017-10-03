using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public GameObject[] _effects;

    #region SINGLETON

    private static EffectManager _instance = null;

    private void Awake()
    {
        Initialize();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Initialize()
    {

    }

    public static EffectManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = Instantiate(Resources.Load("Prefbas/EffectManager") as GameObject).GetComponent<EffectManager>;
        }

        return _instance;
    }

    #endregion
}
