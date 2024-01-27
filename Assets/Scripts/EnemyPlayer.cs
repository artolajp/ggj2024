using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyPlayer : MonoBehaviour
{
    [SerializeField] private float _introTime = 3;
    [SerializeField] private Transform _endPosition;
    [SerializeField] private Transform _eyePosition;
    [SerializeField] private Projectile _eyeProjectile;
    [SerializeField] private float _attackEachTime = 10;
    [SerializeField] private float _lastAttack = 0;

    private Vector3 _startPosition;
    private float _elapsedTime = 0;
    
    
    private void Start()
    {
        _startPosition = transform.position;
        StartCoroutine(MoveToPosition(_endPosition.position, _introTime));
    }

    private void FixedUpdate()
    {
        _elapsedTime += Time.fixedDeltaTime;
        if (_lastAttack + _attackEachTime < _elapsedTime)
        {
            Attack();
            _lastAttack = _elapsedTime;
        }
    }
    
    private void Attack()
    {
        var projectile = Instantiate(_eyeProjectile, _eyePosition.position, new Quaternion());
        projectile.Direction = new Vector2(Random.Range(-1, -0.2f), Random.Range(-1, -0.2f));
    }

    IEnumerator MoveToPosition(Vector3 position, float time)
    {
        while (_elapsedTime < time)
        {
            transform.position = Vector3.Lerp(_startPosition, position, _elapsedTime / time);
            yield return new WaitForEndOfFrame();
            Debug.Log(_elapsedTime);
        }
    }
}
