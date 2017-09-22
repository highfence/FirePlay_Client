using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer = null;
    private Animator _animator = null;

    private Dictionary<CharacterType, string> _playerTypeToAnimPath = new Dictionary<CharacterType, string>()
    {
        { CharacterType.Archer1, "Animator/Archer1" },
        { CharacterType.Archer2, "Animator/Archer2" },
        { CharacterType.Archer3, "Animator/Archer3" }
    };

    private int _fireDirection = 90;
    private bool _isGoesRight = true;
    private bool _isMouseClicked = false;
    private float _fireLineStrength = 0.0f;
    public bool _isEnemy = false;
    public PlayerSpec _spec { get; private set; }

    public void Init(PlayerSpec spec)
    {
        _spec = spec;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        SetAnimator(_spec._playerType);
    }

    private void SetAnimator(CharacterType playerType)
    {
        if (_playerTypeToAnimPath.ContainsKey(playerType) == false)
        {
            Debug.LogAssertionFormat("log");
            return;
        }

        var path = _playerTypeToAnimPath[playerType];
        _animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(path);
    }

    private void Update()
    {
        SpriteControll();
        FireLineControll();
    }

    private void FixedUpdate()
    {
        MoveControll();
        FireDirectionControll();
        FireControll();
    }

    private void FireControll()
    {
        //if (Input.GetKey(KeyCode.Space))
        //{
        //	var fireUnitVec = new Vector2(Mathf.Cos(_fireDirection), Mathf.Sin(_fireDirection));
        //	fireUnitVec.Normalize();

        //	var bulletPrefab = Resources.Load("Prefabs/Bullet") as GameObject;
        //	var bulletInstance = Instantiate<GameObject>(bulletPrefab, transform.position, transform.rotation);

        //	var instanceRigidBody = bulletInstance.GetComponent<Rigidbody2D>();
        //	instanceRigidBody.AddForce(fireUnitVec * 1000);
        //}

        // 0 : 좌클릭 검사.
        if (Input.GetMouseButton(0))
        {
            _isMouseClicked = true;
        }
        else
        {
            if (_isMouseClicked == true)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0.0f;

                FireBullet(mousePosition, transform.position, _fireLineStrength);
            }
            _fireLineStrength = 0.0f;
            _isMouseClicked = false;
        }
    }

    private void FireBullet(Vector3 mouseVec, Vector3 playerVec, float distance)
    {
        // TODO :: 알맞은 총알의 스펙을 읽어다가 생성하기.
        var fireVec = playerVec - mouseVec;
        fireVec.z = 0;
        fireVec.Normalize();

        var bulletPrefab = Resources.Load("Prefabs/Bullet") as GameObject;
        var bulletInstance = Instantiate<GameObject>(bulletPrefab, transform.position, transform.rotation);

        var instanceRigidBody = bulletInstance.GetComponent<Rigidbody2D>();
        instanceRigidBody.AddForce(fireVec * distance * 300);
    }

    private void MoveControll()
    {
        if (_isEnemy)
            return;

        if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            _animator.SetBool("Move", false);
            return;
        }

        _animator.SetBool("Move", true);
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _isGoesRight = false;

            transform.position += Vector3.left * _spec._moveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            _isGoesRight = true;

            transform.position += Vector3.right * _spec._moveSpeed * Time.deltaTime;
        }
    }

    private void FireDirectionControll()
    {
        // 관심있는 입력값이 들어오지 않았다면 바로 return해준다.
        if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
        {
            return;
        }

        // 관심있는 입력값에 대하여 알맞는 처리를 해준다.
        if (Input.GetKey(KeyCode.UpArrow))
        {
            // TODO :: 90도가 아니라 멤버 변수로 처리.
            if (_fireDirection >= _spec._maxFireAngle)
            {
                return;
            }

            ++_fireDirection;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            // TODO :: 위와 마찬가지
            if (_fireDirection <= _spec._minFireAngle)
            {
                return;
            }

            --_fireDirection;
        }
    }

    private void SpriteControll()
    {
        // 오른쪽 왼쪽에 따른 스프라이트 filp 처리.
        if (_isGoesRight)
        {
            _spriteRenderer.flipX = false;
        }
        else
        {
            _spriteRenderer.flipX = true;
        }
    }

    private void FireLineControll()
    {
        // 마우스가 클릭되지 않은 상태면 FireLine을 그리지 않음.
        if (_isMouseClicked == false)
        {
            return;
        }

        // 마우스 위치 구함.
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0.0f;

        _fireLineStrength = Vector3.Distance(mousePosition, transform.position);
        Debug.DrawLine(mousePosition, transform.position, Color.red);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Bullet")
        {
            Debug.Log("Damaged");
            Destroy(coll.gameObject);
        }
    }

    public static class Factory
    {
        public static Player Create(PlayerSpec spec)
        {
            var prefab = Resources.Load("Prefabs/Player") as GameObject;
            var instance = Instantiate(prefab).GetComponent<Player>();

            if (instance == null)
            {
                Debug.LogErrorFormat("Player Instantiate Failed");
                return null;
            }

            instance.Init(spec);

            return instance;
        }
    }
}

public struct PlayerSpec
{
    // Player Type
    [SerializeField]
    public CharacterType _playerType;

    // Move Spec
    [SerializeField]
    public float _moveSpeed;
    [SerializeField]
    public int _moveGauge;

    // Angle Spec
    [SerializeField]
    public int _maxFireAngle;
    [SerializeField]
    public int _minFireAngle;

    // Fire Spec
    [SerializeField]
    public int _firstArmDamage;
    [SerializeField]
    public int _secondArmDamage;

    [SerializeField]
    public float _firstArmRange;
    [SerializeField]
    public float _secondArmRange;

    [SerializeField]
    public float _firstArmWeight;
    [SerializeField]
    public float _secondArmWeight;

    // Life Spec
    public int _maxHealth;

    public static PlayerSpec CreateFromText(string text)
    {
        PlayerSpec instance;
        try
        {
            instance = JsonUtility.FromJson<PlayerSpec>(text);
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("[PlayerSpec] Cannot parse PlayerSpec from source - {0}", text);
            throw;
        }

        return instance;
    }
}