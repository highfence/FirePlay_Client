using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{
    Type1 = 1,
    Type2 = 2,
    Type3 = 3
}

public class Bullet : MonoBehaviour
{
    SpriteRenderer _renderer;
    ExplosionType _type;
    int _damageRatio;

    private void Awake()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
    }

    public void Fire(Vector2 firePosition, Vector2 fireUnitVec, float magnitude, ExplosionType type, int damageRatio)
    {
        _type = type;
        _damageRatio = damageRatio;
        this.transform.position = firePosition;

        var body = this.GetComponent<Rigidbody2D>();
        body.AddForce(fireUnitVec * magnitude);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EffectManager.GetInstance().MakeExplosion(_type, this.transform.position, _damageRatio);

        Destroy(this.gameObject);
    }
}
