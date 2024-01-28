using System;
using UnityEngine;
using UnityEngine.Serialization;
public class Projectile : MonoBehaviour
{
    [SerializeField] private float _projectileSpeed = 1;
    [SerializeField] private float _lifeTime = 2;
    private float _elapsedTime = 0;
    [SerializeField] private float _damage = 2;
    
    [SerializeField] private Vector2 _direction;

    [SerializeField] private bool isBound;
    [SerializeField] private bool isDestroyedByContact;
    [SerializeField] private float _timeToStart = 0;
    
    

    private GameManager _gameManager;

    public int owner = -1;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    public Vector2 Direction
    {
        get => _direction;
        set => _direction = value;
    }
    public float Damage
    {
        get => _damage;
    }
    
    public GameManager GameManager { set => _gameManager = value; }

    private void FixedUpdate()
    {
        _elapsedTime += Time.fixedDeltaTime;
        if (_elapsedTime < _timeToStart)
        {
            _rigidbody.velocity = Vector2.zero;
            return;
        }
        _rigidbody.velocity = Direction.normalized * _projectileSpeed;
        if(_elapsedTime>_lifeTime) GameObject.Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(isBound)
        {
            var contactPoint = other.GetContact(0).point;
            float x = _direction.x;
            float y = _direction.y;

            if (other.gameObject.tag == "wall")
            {
                
                if (contactPoint.x > 0 && _direction.x > 0) x = _direction.x * -1;
                if (contactPoint.x < 0 && _direction.x < 0) x = _direction.x * -1;
            }else if (other.gameObject.tag == "Floor")
            {
                if (contactPoint.y > 0 && _direction.y > 0) y = _direction.y * -1;
                if (contactPoint.y < 0 && _direction.y < 0) y = _direction.y * -1;
            }
            
            _direction = new Vector2(x  ,y);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var playerController = other.gameObject.GetComponent<PlayerController>();
        if (owner<0 && playerController)
        {
            _gameManager.Damage(playerController.PlayerNumber, _damage);
            if(isDestroyedByContact)Destroy(gameObject);
        }
        
        var enemy = other.gameObject.GetComponent<EnemyPlayer>();
        if (owner >= 0 && enemy)
        {
            _gameManager.Damage(-1, _damage);
            if(isDestroyedByContact)Destroy(gameObject);
        }
    }
}
