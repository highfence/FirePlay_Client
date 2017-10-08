using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    GameObject _player = null;
    GameObject _enemy = null;

    #region SINGLETON

    private static EffectManager _instance = null;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public static EffectManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = Instantiate(Resources.Load("Prefabs/EffectManager") as GameObject).GetComponent<EffectManager>();
        }

        return _instance;
    }

    #endregion

    #region FUNCTIONS

    public void SetPlayers(GameObject player, GameObject enemy)
    {
        _player = player;
        _enemy = enemy;
    }

    public void MakeExplosion(ExplosionType type, Vector2 position, int damageRatio)
    {
        if (_player == null || _enemy == null)
            return;

        var explosionObject = Resources.Load("Prefabs/Explosion");
        var instance = Instantiate(explosionObject) as GameObject;

        var explosionPosition = new Vector3(position.x, position.y, 0);
        instance.transform.position = explosionPosition;

        instance.GetComponent<Explosion>().Init(type, _player, _enemy, damageRatio);
    }

    public void FreePlayers()
    {
        _player = null;
        _enemy  = null;
    }

    #endregion
}
