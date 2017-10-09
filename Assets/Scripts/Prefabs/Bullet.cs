using DigitalRuby.Tween;
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
    Vector2 _firePosition;
    SpriteRenderer _renderer;
    ExplosionType _type;
    int _damageRatio;

    private void Awake()
    {
        _renderer = this.GetComponent<SpriteRenderer>();

    }

    private void Update()
    {
        CameraWalk();
    }

    private void CameraWalk()
    {
        float bulletHeight = this.transform.position.y - _firePosition.y;

        if (bulletHeight < 5)
        {
            bulletHeight = 5;
        }

        Camera.main.orthographicSize = bulletHeight;

    }

    public void Fire(Vector2 firePosition, Vector2 fireUnitVec, float magnitude, ExplosionType type, int damageRatio)
    {
        _firePosition = firePosition;
        _type = type;
        _damageRatio = damageRatio;
        this.transform.position = firePosition;

        var body = this.GetComponent<Rigidbody2D>();
        body.AddForce(fireUnitVec * magnitude);

        // Tween 적용.
        Vector3 initPos = new Vector3(_firePosition.x, _firePosition.y, Camera.main.transform.position.z);
        Debug.LogFormat("Fire Magnitude : {0}", magnitude);
        this.gameObject.Tween("BulletFire", 0f, 1f, 1f, TweenScaleFunctions.QuadraticEaseIn, (t) =>
        {
            // Progress
            var cameraPos = initPos;
            cameraPos += new Vector3(this.transform.position.x - initPos.x, this.transform.position.y - initPos.y, 0f) * t.CurrentValue;
            Camera.main.transform.position = cameraPos;
        }, (t) => 
        {
            // Completion
            this.gameObject.Tween("", 0f, 1f, 5f, TweenScaleFunctions.Linear, (t2) =>
            {
                var afterPos = this.transform.position;
                afterPos.z = Camera.main.transform.position.z;
                Camera.main.transform.position = afterPos;
            });
        });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EffectManager.GetInstance().MakeExplosion(_type, this.transform.position, _damageRatio);

        Destroy(this.gameObject);
    }
}
