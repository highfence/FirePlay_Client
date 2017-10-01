using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer = null;
    private Animator       _animator = null;
    private Vector3        _beforePosition;
    private LineRenderer   _fireLine;

    private Dictionary<CharacterType, string> _playerTypeToAnimPath = new Dictionary<CharacterType, string>()
    {
        { CharacterType.Archer1, "Animator/Archer1" },
        { CharacterType.Archer2, "Animator/Archer2" },
        { CharacterType.Archer3, "Animator/Archer3" }
    };

    private bool      _isGoesRight = true;

    private bool      _isEnemy     = false;
    public bool       _isMyTurn    = false;
    public bool       _isMouseDown = false;
    public PlayerSpec _spec { get; private set; }

    public void Init(PlayerSpec spec)
    {
        _beforePosition = transform.position;
        _spec           = spec;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator       = GetComponent<Animator>();
        SetAnimator(_spec._playerType);

        FireLineInitialize();
    }

    public void SetEnemy()
    {
        _isEnemy = true;
        _fireLine.enabled = false;
    }

    private void FireLineInitialize()
    {
        _fireLine = GetComponent<LineRenderer>();

        // Fire Line 색 설정.
        _fireLine.startColor = Color.red;
        _fireLine.endColor = Color.yellow;

        // Fire Line 두께 설정.
        _fireLine.startWidth = 0.1f;
        _fireLine.endWidth = 0.1f;

        // 라인 시작점 설정.
        _fireLine.SetPosition(0, transform.position);
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

        // 조작 관련 함수들.
        FireControll();
        MoveControll();
        KeyUpDetect();
    }

    private void MoveControll()
    {
        if (_isEnemy || _isMouseDown == false)
        {
            return;
        }

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

    private void KeyUpDetect()
    {
        if (_isEnemy || _isMouseDown == false)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            Debug.Log("Key Up Command Detected");

            // 손가락을 떼었을 때.
            var curPosition = transform.position;

            var movedRange = _beforePosition.x - curPosition.x;

            _beforePosition = curPosition;

            // TODO :: 서버에서 이해할 수 있는 좌표 값으로 바꿔서 보내주어야 함.
            // TODO :: MoveNotify와 Ack의 포지션을 float값으로 바꾸어야 함.
            var moveNotify = new PacketInfo.MoveNotify()
            {
                _enemyPositionX = (int)transform.position.x,
                _enemyPositionY = (int)transform.position.y,
                _moveRange = (int)((Camera.main.ViewportToWorldPoint(new Vector3(movedRange, 0.0f, 0.0f)).x))
            };

            NetworkManager.GetInstance().SendPacket<PacketInfo.MoveNotify>(moveNotify, PacketInfo.PacketId.ID_MoveNotify);
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

    private void FireControll()
    {
        _fireLine.SetPosition(0, transform.position);

        if (_isEnemy || _isMyTurn == false)
            return;
        
        // 클릭 관련 검사.
        if (Input.GetMouseButtonDown(0))
        {
            _isMouseDown = true;
        }
        else
        {
            if (_isMouseDown == true)
            {
                _fireLine.enabled = true;
                _fireLine.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isMouseDown = false;
            _fireLine.enabled = false;

            // 발사.
            FireBullet(Input.mousePosition);            
        }
    }

    private void FireBullet(Vector3 mousePosition)
    {
        var unitVec3 = this.transform.position - mousePosition;
        var unitVec2 = new Vector2(unitVec3.x, unitVec3.y);
        var magnitude = unitVec2.magnitude;
        unitVec2.Normalize();

        var bullet = Instantiate(Resources.Load("Prefabs/Bullet")) as GameObject;
        bullet.GetComponent<Bullet>().Fire(this.gameObject, unitVec2, magnitude);

        // 서버에 발사했다고 알림.
        var fireNotify = new PacketInfo.FireNotify()
        {

        };
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