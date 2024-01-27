using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{    
    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private float _jumpForce = 350;
    [SerializeField] private Vector2 _jumpWallForce = new Vector2(250,250);
    [SerializeField] private float floorDistance = 0.6f;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _mainSprite;
    [SerializeField] private Projectile _projectile;
    [SerializeField] private Transform _projectileTransform;
    [SerializeField] private float _startLife;
    [SerializeField] private float _currentLife;
    

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpAudio;
    [SerializeField] private AudioClip hitAudio;
    
    private bool _actioning;
    private bool _lastActioning;
    
    private bool _actioningJump;
    private bool _lastActioningJump;
    
    private Rigidbody2D _rigidbody2D;
    private Vector2 _lastDirection;

    private bool _moving;
    private bool _onFloor;
    private bool _onLeftWall;
    private bool _onRightWall;
    private bool _inputEnable;
    private bool _isFalling;
    private bool _isJumping;
    public float Score { get; set; }
    public int PlayerNumber { get; protected set; }

    private List<PlayerController> _targets;

    private GameManager _gameManager;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Initialize(int playerNumber, GameManager gameManager)
    {
        _inputEnable = true;
        Score = 0;
        PlayerNumber = playerNumber;
        _targets = new List<PlayerController>();
        _gameManager = gameManager;
        _isFalling = false;
        _isJumping = false;
    }

    private void Update()
    {
        _onFloor = IsRaycastWith(transform.position + Vector3.right * 0.3f, Vector3.down + Vector3.right * 0.3f, "Floor");
        _onFloor |= IsRaycastWith(transform.position + Vector3.left * 0.3f, Vector3.down - Vector3.left * 0.3f, "Floor");
        _onRightWall = IsRaycastWith(transform.position, Vector3.right, "Floor");
        _onRightWall |= IsRaycastWith(transform.position + Vector3.down * 0.3f, Vector3.right + Vector3.down * 0.3f, "Floor");
        _onLeftWall = IsRaycastWith(transform.position, Vector3.left, "Floor");
        _onLeftWall |= IsRaycastWith(transform.position + Vector3.down * 0.3f, Vector3.left + Vector3.down * 0.3f, "Floor");
        if (_onFloor)
        {
            _animator.SetBool("isJumping",false);
        }
        if (_isJumping)
        {
            _rigidbody2D.excludeLayers = LayerMask.GetMask("Floor");
            
        }
        else
        {
            _rigidbody2D.excludeLayers = 0;

        }
    }

    private bool IsRaycastWith(Vector2 origin, Vector2 direction, string tag)
    {
        bool isRaycast = false;
        var casts = Physics2D.RaycastAll(origin,  direction, floorDistance);
        foreach (var hit2D in casts)
        {
            isRaycast |= hit2D.collider.tag == tag;
        }

        return isRaycast;
    }

    private void FixedUpdate() {
        if (!_moving )
        {
            float breakSpeed = _onFloor ? 0.5f : 0.95f;
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x * breakSpeed, _rigidbody2D.velocity.y);
        } else if(_inputEnable){
            _moving = false;            
            _animator.SetBool("isWalking",false);
        }
        
        if (!_lastActioning && _actioning) {
            _actioning = false;            
            _animator.SetBool("isAttaking",false);
        }
        _lastActioning = false;
        
        if (!_lastActioningJump && _actioningJump) {
            _actioningJump = false;
        }
        _lastActioningJump = false;

        _isFalling = _rigidbody2D.velocity.y < 0;
        _isJumping = _rigidbody2D.velocity.y > 0;

       
    }

    public void Move(Vector2 direction) {
        if (!_inputEnable) return;
        
        _animator.SetBool("isWalking",true);
        _moving = true;
        Vector2 velocity = new Vector2(direction.normalized.x * _speed, _rigidbody2D.velocity.y);
        _rigidbody2D.velocity = velocity;

        _mainSprite.gameObject.transform.localScale = velocity.x > 0? Vector3.one: new Vector3(-1,1,1);

        _lastDirection = direction;
    }

    public void Action() {
        _lastActioning = true;

        if (!_inputEnable) return;
        
        if (!_actioning) {
            _actioning = true;
            Attack();
        } 
    }

    private void Attack()
    {
        
        // foreach (PlayerController player in _targets)
        // {
        //     _gameManager.Attack(this, player);
        // }

        var projectilePostion =  Vector3.zero + (_projectileTransform.position);
        projectilePostion.x = _mainSprite.gameObject.transform.localScale.x < 0 ?
            2*transform.position.x-projectilePostion.x: 
            projectilePostion.x; 

        var projectile = Instantiate(_projectile, projectilePostion, new Quaternion());
        projectile.Direction = _lastDirection != Vector2.zero ? _lastDirection.normalized : Vector3.right * _mainSprite.gameObject.transform.localScale.x;
        _animator.SetBool("isAttaking",true);
        PlayHitAudio();
    }

    public void Jump() {
        _lastActioningJump = true;
        if (!_inputEnable) return;

        if (!_actioningJump && !_isJumping) {
            if (_onFloor) {
                _actioningJump = true;
                _rigidbody2D.AddForce(new Vector2(0, _jumpForce));
                _animator.SetBool("isJumping", true);
                PlayJumpAudio();

            } else if (_onRightWall) {
                _actioningJump = true;
                _rigidbody2D.velocity = Vector2.zero;
                _rigidbody2D.AddForce(_jumpWallForce * new Vector2(-1, 1));
                DisableInputs();
                _animator.SetBool("isJumping", true);
                PlayJumpAudio();

            } else if (_onLeftWall) {
                _actioningJump = true;
                _rigidbody2D.velocity = Vector2.zero;
                _rigidbody2D.AddForce(_jumpWallForce * new Vector2(1, 1));
                DisableInputs();
                _animator.SetBool("isJumping", true);
                PlayJumpAudio();
            }
        }

    }

    private void PlayJumpAudio() {
        audioSource.PlayOneShot(jumpAudio);
    }

    public void PlayHitAudio() {
        audioSource.PlayOneShot(hitAudio);
    }

    public void DisableInputs()
    {
        _inputEnable = false;
        Invoke(nameof(EnableInputs), 0.3f);
    }
    
    public void EnableInputs()
    {
        _inputEnable = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var target = other.GetComponent<PlayerController>();
        if (target)
        {
            _targets.Add(target);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        var target = other.GetComponent<PlayerController>();
        if (target)
        {
            _targets.Remove(target);
        }
    }

    public void Push(Vector3 origin, float pushForce)
    {
        Vector3 normalizedDirection = (transform.position-origin).normalized;
        normalizedDirection.y = Mathf.Abs(normalizedDirection.y);
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.AddForce(normalizedDirection * pushForce);

        DisableInputs();
    }
    
    public void ReceiveDamage(PlayerController player)
    {
        _currentLife -= player._projectile.Damage;
    }
}