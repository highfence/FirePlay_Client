using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    SpriteRenderer _renderer;
    ExplosionType _type;

    private void Awake()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
    }

    public void Fire(Vector2 firePosition, Vector2 fireUnitVec, float magnitude, ExplosionType type)
    {
        _type = type;
        this.transform.position = firePosition;

        var body = this.GetComponent<Rigidbody2D>();
        body.AddForce(fireUnitVec * magnitude);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EffectManager.GetInstance().MakeExplosion(_type, this.transform.position);

        Destroy(this.gameObject);
    }
}
