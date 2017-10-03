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

    private void Init(ExplosionType type)
    {

    }

    private struct ExplosionSpec
    {
        [SerializeField]
        float _range;

        [SerializeField]
        int _minDamage;
        int _maxDamage;

        [SerializeField]
        string _spritePath;

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

