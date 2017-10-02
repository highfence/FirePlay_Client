﻿using System.Collections;
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

    public void Fire(GameObject owner, Vector2 firePosition, Vector2 fireUnitVec, float magnitude)
    {
        _owner = owner;
        this.transform.position = firePosition;

        var body = this.GetComponent<Rigidbody2D>();
        body.AddForce(fireUnitVec * magnitude);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 쏜 사람은 자기 총알에 맞지 않음.
        //if (collision.gameObject == _owner)
        //    return;

        var explosion = Instantiate(Resources.Load("Prefabs/Explosion") as GameObject);
        explosion.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        explosion.transform.position = this.transform.position;

        Destroy(this.gameObject);
    }

}
