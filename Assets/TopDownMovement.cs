using System.Collections.Generic;
using UnityEngine;

public class TopDownMovement : MonoBehaviour
{
    [SerializeField] private float _velocityMultiplier = 2f;
    [SerializeField] private float _minMoveDistance = 0.001f;
    [SerializeField] private float _minDistanceToCollider = 0.02f;
    [SerializeField] private float _shellRadius = 0.01f;
    [SerializeField] private float _animationSpeedMultiplier = 0.1f;

    public Vector2 Velocity;
    public LayerMask LayerMask;

    private Vector2 _targetVelocity;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidBody2D;
    private Animator _animator;
    private ContactFilter2D _contactFilter;
    private RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];
    private List<RaycastHit2D> _hitBufferList = new List<RaycastHit2D>(16);

    private void OnEnable()
    {
        _animator = GetComponent<Animator>();
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _contactFilter.useTriggers = false;
        _contactFilter.SetLayerMask(LayerMask);
        _contactFilter.useLayerMask = true;
    }

    private void Update()
    {
        _targetVelocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        Velocity = _targetVelocity * _velocityMultiplier;

        Vector2 moveVelocity = Velocity * Time.deltaTime;

        if (moveVelocity.x > 0)
        {
            _spriteRenderer.flipX = false;
        }
        else if (moveVelocity.x < 0)
        {
            _spriteRenderer.flipX = true;
        }

        Movement(moveVelocity);
    }

    private void Movement(Vector2 move)
    {
        float distance = move.normalized.magnitude * _velocityMultiplier; 

        if (distance > _minMoveDistance)
        {
            int count = _rigidBody2D.Cast(move, _contactFilter, _hitBuffer, distance + _shellRadius);

            _hitBufferList.Clear();

            for (int i = 0; i < count; i++)
            {
                _hitBufferList.Add(_hitBuffer[i]); 
            }

            for (int i = 0; i < _hitBufferList.Count; i++)
            {
                Vector2 currentNormal = _hitBufferList[i].normal;
                float velocityToColliderNormalProjection = Vector2.Dot(currentNormal, Velocity);

                if (velocityToColliderNormalProjection < 0)
                {
                    Velocity -= velocityToColliderNormalProjection * currentNormal;
                }

                float modifiedDistance = _hitBufferList[i].distance - _shellRadius;

                if (modifiedDistance < _minDistanceToCollider)
                {
                    _rigidBody2D.position += currentNormal * _minDistanceToCollider;

                    Vector2 colliderGround = Turn90Degree(currentNormal);
                    float velocityToColliderGroundProjection = Vector2.Dot(colliderGround, Velocity);

                    move = colliderGround * velocityToColliderGroundProjection;
                }
                
                if (modifiedDistance < distance)
                {
                    distance = modifiedDistance;
                }
            }

            _rigidBody2D.position += move.normalized * distance;
        }

        float speed = move.normalized.magnitude * distance * _animationSpeedMultiplier;
        _animator.SetFloat("Speed", speed);
    }

    private Vector2 Turn90Degree(Vector2 vector)
    {
        Vector2 newVector = new Vector2
        {
            y = -vector.x,
            x = vector.y
        };

        return newVector;
    }
}