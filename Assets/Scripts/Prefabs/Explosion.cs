using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExplosionType
{
    Type1,
    Type2,
    Type3
}

public class Explosion : MonoBehaviour
{
    private Dictionary<ExplosionType, string> _explosionTypeToSpecPath = new Dictionary<ExplosionType, string>()
    {
        { ExplosionType.Type1, "Data/Type1" },
        { ExplosionType.Type2, "Data/Type2" },
        { ExplosionType.Type3, "Data/Type3" }
    };

    private ExplosionSpec _spec;
    private GameObject _explosion;

    public void Init(ExplosionType explosionType)
    {
        #region SPEC LOAD

        if (_explosionTypeToSpecPath.ContainsKey(explosionType) == false)
        {
            Debug.LogAssertionFormat("Invalid Dictionay Data Path - {0]", explosionType);
            return;
        }

        var path = _explosionTypeToSpecPath[explosionType];
        var text = Resources.Load<TextAsset>(path).text;
        _spec = ExplosionSpec.CreateFromText(text);

        #endregion

        #region MAKE EFFECT

        _explosion = Instantiate(Resources.Load(_spec._prefabPath)) as GameObject;
        _explosion.transform.position = this.transform.position;
        _explosion.transform.SetParent(this.transform);

        #endregion

        #region COLLISION DETECT 

         

        #endregion

        #region DELETE SELF

        Destroy(this.gameObject, 3f);

        #endregion
    }

    private struct ExplosionSpec
    {
        [SerializeField]
        public float _range;

        [SerializeField]
        public int _minDamage;
        public int _maxDamage;

        [SerializeField]
        public string _prefabPath;

        public static ExplosionSpec CreateFromText(string text)
        {
            ExplosionSpec instance;
            try
            {
                instance = JsonUtility.FromJson<ExplosionSpec>(text);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("[ExplosionSpec] Cannot make instance from text - {0}, {1}", text, e.Message);
                throw;
            }

            return instance;
        }
    }
}

