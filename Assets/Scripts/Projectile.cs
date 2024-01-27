using UnityEngine;
public class Projectile : MonoBehaviour
{
    [SerializeField] private float _projectileSpeed = 1;
    [SerializeField] private float _lifeTime = 2;
    
    [SerializeField] private Vector2 _direction;

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

    private void FixedUpdate()
    {
        _rigidbody.velocity = Direction.normalized * _projectileSpeed; 
    }
}
