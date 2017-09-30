using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    SpriteRenderer _renderer;
    GameObject _owner;

    private void Awake()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
    }

    private void Fire(GameObject owner, Vector2 fireUnitVec, float magnitude)
    {
        _owner = owner;

        var body = this.GetComponent<Rigidbody2D>();
        body.AddForce(fireUnitVec * magnitude);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == _owner)
            return;
        
    }

}
