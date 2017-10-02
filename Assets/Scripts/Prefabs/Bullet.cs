using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
    }

    public void Fire(Vector2 firePosition, Vector2 fireUnitVec, float magnitude)
    {
        this.transform.position = firePosition;

        var body = this.GetComponent<Rigidbody2D>();
        body.AddForce(fireUnitVec * magnitude);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var explosion = Instantiate(Resources.Load("Prefabs/Explosion") as GameObject);
        explosion.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        explosion.transform.position = this.transform.position;

        Destroy(this.gameObject);
    }

}
