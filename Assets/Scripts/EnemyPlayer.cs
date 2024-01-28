using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemyPlayer : MonoBehaviour
{
    [SerializeField] private float _startLife = 100;
    [SerializeField] private float _currentLife;
    [SerializeField] private float _introTime = 3;
    [SerializeField] private Transform _endPosition;
    [SerializeField] private Transform _eyePosition;
    [SerializeField] private Transform _dogPosition;
    [SerializeField] private Projectile _eyeProjectile;
    [SerializeField] private Projectile _dogProjectile;
    [SerializeField] private float _attack1EachTime = 10;
    [SerializeField] private float _attack2EachTime = 10;
    [FormerlySerializedAs("_lastAttack")]
    [SerializeField] private float _lastAttack1 = 15;
    [SerializeField] private float _lastAttack2 = 8;
    [SerializeField] private GameManager _gameManager;
    

    private Vector3 _startPosition;
    private float _elapsedTime = 0;
    private Animator _animator;
    private SpriteRenderer spriteRenderer;
    
    [SerializeField] private Color _targetDeadColor = Color.red;

    public bool IsAlive
    {
        get => _currentLife > 0;
    }
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _startPosition = transform.position;
        StartCoroutine(MoveToPosition(_endPosition.position, _introTime));
    }

    private void Update()
    {
        if (IsAlive)
        {
            
            spriteRenderer.color = Color.Lerp(_targetDeadColor, Color.white, _currentLife / _startLife);
        }
        else
        {

            spriteRenderer.color = Color.white;
        }
    }

    private void FixedUpdate()
    {
        _elapsedTime += Time.fixedDeltaTime;
        if (_lastAttack1 + _attack1EachTime < _elapsedTime)
        {
            Attack(1);
            _lastAttack1 = _elapsedTime;
        }
        
        if (_lastAttack2 + _attack2EachTime < _elapsedTime)
        {
            Attack(2);
            _lastAttack2 = _elapsedTime;
        }
    }
    
    private void Attack(int attack)
    {
        if (attack == 1)
        {
            _animator.SetBool("isAttack1", true);
            Invoke(nameof(StartAttack1), 1);
        }
        else
        {
            var projectile = Instantiate(_dogProjectile, _dogPosition.position, new Quaternion());
            projectile.Direction = Vector2.left;
            projectile.owner = -1;
            projectile.GameManager = _gameManager;
            _gameManager.PlayDogMusic();
            
            _lastAttack2 = _elapsedTime;
        }
    }

    private void StartAttack1()
    {
        var projectile = Instantiate(_eyeProjectile, _eyePosition.position, new Quaternion());
        projectile.Direction = new Vector2(Random.Range(-1, -0.2f), Random.Range(-1, -0.2f));
        Invoke(nameof(StopAttack), 1);
        projectile.owner = -1;
        projectile.GameManager = _gameManager;

    }
    
    private void StopAttack()
    {
        _animator.SetBool("isAttack1", false);
    }

    IEnumerator MoveToPosition(Vector3 position, float time)
    {
        _animator.SetBool("isTranformed", false);
        _animator.SetBool("isAttack1", false);
        while (_elapsedTime < time)
        {
            transform.position = Vector3.Lerp(_startPosition, position, _elapsedTime / time);
            yield return new WaitForEndOfFrame();
        }
        _animator.SetBool("isTranformed", true);
    }
    
    public void ReceiveDamage(float value)
    {
        _currentLife -= value;
    }
}
