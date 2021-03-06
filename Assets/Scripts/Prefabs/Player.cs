﻿using DG.Tweening;
using DigitalRuby.Tween;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 데미지를 받았을 때 호출할 이벤트.
    public event Action OnDamageOccured = delegate { };

    [SerializeField]
    private SpriteRenderer _spriteRenderer = null;
    private GameObject     _crosshair      = null;
    private Animator       _animator       = null;
    private Vector3        _beforePosition;
    private LineRenderer   _fireLine;
    private GameObject     _healthBar      = null;
    public  int            _hp             = 0;
    public  GameObject     _hpParticle     = null;

    private Dictionary<CharacterType, string> _playerTypeToAnimPath = new Dictionary<CharacterType, string>()
    {
        { CharacterType.Archer1, "Animator/Archer1" },
        { CharacterType.Archer2, "Animator/Archer2" },
        { CharacterType.Archer3, "Animator/Archer3" }
    };

    private Dictionary<CharacterType, string> _playerTypeToSpritePath = new Dictionary<CharacterType, string>()
    {
        { CharacterType.Archer1, "PrivateData/SpritesArchers/Archer1/FantasyArcher_01_Attack1_0213" },
        { CharacterType.Archer2, "PrivateData/SpritesArchers/Archer2/FantasyArcher_02_Attack1_0219" },
        { CharacterType.Archer3, "PrivateData/SpritesArchers/Archer3/FantasyArcher_03_Attack1_0211" }
    };

    private bool      _isGoesRight = true;
    private bool      _isEnemy     = false;
    public bool       _isMyTurn    = false;
    public bool       _isMouseDown = false;
    public bool       _isMoving    = false;
    public PlayerSpec _spec { get; private set; }

    public void Init(PlayerSpec spec)
    {
        _beforePosition = transform.position;
        _spec           = spec;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator       = GetComponent<Animator>();
        _hp             = spec._maxHealth;

        SetAnimator(_spec._playerType);
        SetSprite(_spec._playerType);

        FireLineInitialize();
        CrosshairInitialize();

        this.gameObject.AddComponent<PolygonCollider2D>();
    }

    private void CrosshairInitialize()
    {
        try
        {
            _crosshair = Instantiate(Resources.Load("Prefabs/FireCrosshair") as GameObject);
            _crosshair.transform.SetParent(this.transform);
            _crosshair.GetComponent<SpriteRenderer>().enabled = false;
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void SetEnemy()
    {
        _isEnemy = true;
        _fireLine.enabled = false;
    }

    public void Damaged(int damage)
    {
        _hp = _hp - damage;

        var remainRatio = (float)_hp / (float)_spec._maxHealth;

        _healthBar.GetComponent<ProgressBarPro>().Value = remainRatio;

        GameObject damageParticle = Instantiate(_hpParticle, this.transform.position, this.gameObject.transform.rotation) as GameObject;
        damageParticle.GetComponent<AlwaysFace>().Target = GameObject.Find("Main Camera").gameObject;

        TextMesh textMesh = damageParticle.transform.Find("HPLabel").GetComponent<TextMesh>();
        textMesh.text = "-" + damage.ToString();

        _animator.SetTrigger("Damage");

        OnDamageOccured.Invoke();
    }

    public void SetHealthBar(GameObject healthBar)
    {
        _healthBar = healthBar;
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

        _fireLine.SetPosition(0, (transform.position));
        _fireLine.enabled = false;
    }

    private void SetAnimator(CharacterType playerType)
    {
        if (_playerTypeToAnimPath.ContainsKey(playerType) == false)
        {
            Debug.LogAssertionFormat("Invalid Dictionary Anim Path - {0}", playerType);
            return;
        }

        var path = _playerTypeToAnimPath[playerType];
        _animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(path);
    }

    private void SetSprite(CharacterType playerType)
    {
        if (_playerTypeToSpritePath.ContainsKey(playerType) == false)
        {
            Debug.LogAssertionFormat("Invalid Dictionary Sprite Path - {0}", playerType);
            return;
        }

        var path = _playerTypeToSpritePath[playerType];
        _spriteRenderer.sprite = Resources.Load<Sprite>(path);
    }

    private void Update()
    {
        SpriteControll();

        // 조작 관련 함수들.
        MouseControll();
        MoveControll();
        KeyUpDetect();
        TurnControll();

        // 카메라 움직임.
        CameraMove();
    }

    private void CameraMove()
    {
        if (_isEnemy || _isMyTurn == false || _isMoving == false)
            return;

        var playerCurrentPos = Camera.main.transform.position;
        playerCurrentPos.x = this.transform.position.x;
        playerCurrentPos.y = this.transform.position.y;
        Camera.main.transform.position = playerCurrentPos;
    }

    private void TurnControll()
    {
        if (_isMyTurn == false)
            return;

        // 임시로 Esc를 누르면 턴이 바뀌도록 만들어놓음.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var endNotify = new PacketInfo.TurnEndNotify();

            NetworkManager.GetInstance().SendPacket<PacketInfo.TurnEndNotify>(endNotify, PacketInfo.PacketId.ID_TurnEndNotify);
        }
    }

    private void MoveControll()
    {
        if (_isEnemy == true || _isMyTurn == false || _isMouseDown == true)
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
            _isMoving = true;

            transform.position += Vector3.left * _spec._moveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            _isGoesRight = true;
            _isMoving = true;

            transform.position += Vector3.right * _spec._moveSpeed * Time.deltaTime;
        }
    }

    private void KeyUpDetect()
    {
        if (_isEnemy || _isMyTurn == false || _isMouseDown == true)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            // 손가락을 떼었을 때.
            var curPosition = transform.position;

            var movedRange = _beforePosition.x - curPosition.x;

            _beforePosition = curPosition;

            var moveNotify = new PacketInfo.MoveNotify()
            {
                _enemyPositionX = this.transform.position.x,
                _enemyPositionY = this.transform.position.y
            };

            NetworkManager.GetInstance().SendPacket(moveNotify, PacketInfo.PacketId.ID_MoveNotify);
            DataContainer.GetInstance().SetPlayerPosition(this.transform.position);

            _isMoving = false;
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

    private void MouseControll()
    {
        _fireLine.SetPosition(0, (transform.position));

        if (_isEnemy || _isMyTurn == false || _isMoving)
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
                #region FIRELINE CONTROLL

                _fireLine.enabled = true;
                _fireLine.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));

                #endregion

                #region CROSSHAIR CONTROLL

                _crosshair.GetComponent<SpriteRenderer>().enabled = true;
                var playerPos = new Vector2(this.transform.position.x, this.transform.position.y);
                var mouseWorldPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
                var oppositeUnitVec = playerPos - mouseWorldPos;
                oppositeUnitVec.Normalize();
                oppositeUnitVec *= 3f;

                _crosshair.transform.position = (this.transform.position + new Vector3(oppositeUnitVec.x, oppositeUnitVec.y, 0));

                #endregion
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            // 발사.
            StartCoroutine("OnAttackStarted");

            _isMouseDown = false;
            _fireLine.enabled = false;
            _crosshair.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public IEnumerator OnAttackStarted()
    {
        // 내 턴을 끝낸다.
        _isMyTurn = false;

        var mousePosition = Input.mousePosition;
        var crosshairPosition = _crosshair.transform.position;

        #region WAIT FOR MOVE END

        while (true)
        {
            if (_isMoving == true)
                continue;

            break;
        }

        #endregion

        #region START ATTACK ANIMATION

        if (mousePosition.x > this.transform.position.x)
        {
            _isGoesRight = false;
            _spriteRenderer.flipX = true;
        }
        else if (mousePosition.x < this.transform.position.x)
        {
            _isGoesRight = true;
            _spriteRenderer.flipX = false;
        }

        _animator.SetTrigger("Attack");

        // 애니메이션이 끝날 때까지 기다림.
        yield return new WaitForSeconds(1);

        #endregion

        var unitVec3 = Camera.main.WorldToScreenPoint(this.transform.position) - mousePosition;
        var unitVec2 = new Vector2(unitVec3.x, unitVec3.y);
        var magnitude = unitVec2.magnitude;

        if (magnitude > 1000)
        {
            Debug.LogWarningFormat("Much Magnitude! : {0}", magnitude);
            magnitude = 1000;
        }

        unitVec2.Normalize();

        // 데미지 결정.
        var damageRatio = (int)UnityEngine.Random.Range(0, 100);

        // 총알 발사.
        var bullet = Instantiate(Resources.Load("Prefabs/Bullet")) as GameObject;
        bullet.GetComponent<Bullet>().Fire(crosshairPosition, unitVec2, magnitude * 2, ExplosionType.Type1, damageRatio);

        // 서버에 발사했다고 알림.
        var fireNotify = new PacketInfo.FireNotify()
        {
            _fireType = 0,
            _enemyPositionX = this.transform.position.x,
            _damage = damageRatio,
            _magnitude = magnitude,
            _unitVecX = unitVec2.x,
            _unitVecY = unitVec2.y
        };

        NetworkManager.GetInstance().SendPacket(fireNotify, PacketInfo.PacketId.ID_FireNotify);
    }

    public IEnumerator OnMoveCommanded(float destPositionX)
    {
        #region MOVE ANIM SETTING

        _isMoving = true;
        _animator.SetBool("Move", true);

        if (destPositionX > this.transform.position.x)
        {
            _isGoesRight = true;
        }
        else if (destPositionX < this.transform.position.x)
        {
            _isGoesRight = false;
        }

        #endregion

        // 1초마다 움직일 거리 계산.
        float unitMoveRange = _spec._moveSpeed / 2f;
        float remainDistance = 0f;

        if (_isGoesRight == true)
        {
            remainDistance = destPositionX - this.transform.position.x;
        }
        else
        {
            remainDistance = this.transform.position.x - destPositionX;
        }

        var neededTimeToMove = remainDistance / unitMoveRange;
        transform.DOMoveX(destPositionX, neededTimeToMove);

        var playerCurrentPos = Camera.main.transform.position;
        playerCurrentPos.x = this.transform.position.x;
        playerCurrentPos.y = this.transform.position.y;
        Camera.main.transform.DOMove(playerCurrentPos, 0.25f);

        yield return new WaitForSeconds(neededTimeToMove);

        _animator.SetBool("Move", false);
    }

    public IEnumerator OnEnemyAttackStarted(PacketInfo.EnemyFireNotify enemyFireInfo)
    {
        var unitVec = new Vector2(enemyFireInfo._unitVecX, enemyFireInfo._unitVecY);
        var magnitude = enemyFireInfo._magnitude;

        #region MAKE ENEMY CROSSHAIR

        _crosshair.GetComponent<SpriteRenderer>().enabled = true;
        var enemyPos = new Vector2(this.transform.position.x, this.transform.position.y);
        var crosshairVec = unitVec;
        crosshairVec *= 3;

        _crosshair.transform.position = (this.transform.position + new Vector3(crosshairVec.x, crosshairVec.y, 0));

        #endregion

        #region START ATTACK ANIMATION

        _animator.SetTrigger("Attack");

        // 애니메이션이 끝날 때까지 기다림.
        yield return new WaitForSeconds(1);

        #endregion

        // 총알 발사.
        var bullet = Instantiate(Resources.Load("Prefabs/Bullet")) as GameObject;
        bullet.GetComponent<Bullet>().Fire(_crosshair.transform.position, unitVec, magnitude * 2, ExplosionType.Type1, enemyFireInfo._damage);

        _crosshair.GetComponent<SpriteRenderer>().enabled = false;

        // 잘 받았다고 서버에 알림.
        var enemyFireAck = new PacketInfo.EnemyFireAck()
        {
            _result = (int)ErrorCode.None
        };

        NetworkManager.GetInstance().SendPacket(enemyFireAck, PacketInfo.PacketId.ID_EnemyFireAck);
    }

    public void TurnEndSetting()
    {
        if (_isMyTurn == false)
            return;

        _isMyTurn = false;
        _isMoving = false;
        _isMouseDown = false;
        _fireLine.enabled = false;
        _crosshair.GetComponent<SpriteRenderer>().enabled = false;
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

    // Angle Spec
    [SerializeField]
    public int _maxFireAngle;
    [SerializeField]
    public int _minFireAngle;

    // Fire Spec
    [SerializeField]
    public BulletType _bulletType;
    [SerializeField]
    public float _armWeight;

    // Life Spec
    [SerializeField]
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
            Debug.LogErrorFormat("[PlayerSpec] Cannot parse PlayerSpec from source - {0}, {1}", text, e.Message);
            throw;
        }

        return instance;
    }
}
