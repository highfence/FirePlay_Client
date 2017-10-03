using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
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
            _instance = Instantiate(Resources.Load("Prefbas/EffectManager") as GameObject).GetComponent<EffectManager>();
        }

        return _instance;
    }

    #endregion

    #region FUNCTIONS

    public void MakeExplosion(ExplosionType type, Vector2 position)
    {
        var explosionObject = Resources.Load("Prefabs/Explosion");
        var instance = Instantiate(explosionObject) as GameObject;
        instance.GetComponent<Explosion>().Init(type);

        var explosionPosition = new Vector3(position.x, position.y, 0);
        instance.transform.position = explosionPosition;
    }

    #endregion
}
