using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public abstract class Creatures : RaycastController
{

    [SerializeField] protected float _movementSpeed;
    [SerializeField] protected float _attackSpeed;
    [SerializeField] protected float _jumpPower;
    [SerializeField] protected bool _hostile;
    protected bool _canAttack;
    protected bool _attacking;
    protected Vector2 _facing = Vector2.zero; //0 = right, 1 = left, 2 = up, 3 = down 
    public Vector2 GetFacing { get { return _facing; } }
    public Vector2 SetFacing { set { _facing = value; } }
    public bool CanAttack { get { return _canAttack; } }
    protected Rigidbody2D _body;
    protected Dictionary<Creatures, float> _ignoreHit;
    public Rigidbody2D Body { get { return _body; } }
    public float MovementSpeed { get { return _movementSpeed; } }
    public float JumpPower { get { return _jumpPower; } }
    public float AttackSpeed { get { return _attackSpeed; } }
    private Health _health;
    public Health Health { get { return _health; } }
    protected bool _canJump;
    protected bool _falling;
    public bool CanJump { get { return _canJump; } }
    public bool Falling { get { return _falling; } }
    protected Vector2 _direction;
    public Vector2 Direction { get { return _direction; } set { _direction = value; } }
    protected BoxCollider2D _hitBox;
    protected float _fallMultipler;
    [SerializeField] private GameObject _meleeAttackObject;
    protected MeleeAttack _meleeAttack;
    private SpriteRenderer _renderer;
    float maxClimbSlope = 80f;
    float maxDescendAngle = 75f;
    protected CollisionInfo _collisions;
    public CollisionInfo Collisions { get { return _collisions; } }

    void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _meleeAttack = _meleeAttackObject.GetComponent<MeleeAttack>();
        _falling = false;
        _canAttack = true;
        _renderer = GetComponent<SpriteRenderer>();
        _collisions = new CollisionInfo();
        _hitBox = gameObject.GetComponent<BoxCollider2D>();
        _ignoreHit = new Dictionary<Creatures, float>();
        _canJump = true;
        _fallMultipler = 2.5f;
        _attacking = false;
        _facing = Vector2.right;
        _health = GetComponent<Health>();
        _health.RegisterDeathMethod(OnDeath);
    }

    public void Move(Vector3 velocity, bool standingOnPlatform = false)
    {

        UpdateRaycastOrigins();
        _collisions.Reset();
        _collisions.velocityOld = velocity;
        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }
        if (velocity.x != 0f)
        {
            HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0f)
        {
            VerticleCollisions(ref velocity);
        }
        transform.Translate(velocity);
        if (standingOnPlatform)
        {
            _collisions.below = true;
        }
    }

    protected void OnDeath()
    {
        Debug.Log("We died.");
        Destroy(gameObject);
    }

    private void HorizontalCollisions(ref Vector3 velocity)
    {
        float dirX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (dirX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * dirX * rayLength, Color.blue);
            if (hit)
            {
                if (hit.distance == 0)
                {
                    continue;
                }
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && slopeAngle <= maxClimbSlope)
                {
                    if (_collisions.descendingSlope)
                    {
                        _collisions.descendingSlope = false;
                        velocity = _collisions.velocityOld;
                    }
                    float distanceToSlopeStart = 0f;
                    if (slopeAngle != _collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart * dirX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * dirX;
                    Debug.Log("Climb slope");
                }
                Debug.Log("Hit something!");
                Debug.Log(slopeAngle);
                if (!_collisions.climbingSlope || slopeAngle > maxClimbSlope)
                {
                    velocity.x = (hit.distance - skinWidth) * dirX;
                    rayLength = hit.distance;

                    if (_collisions.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(_collisions.slopeAngle * Mathf.Deg2Rad * Mathf.Abs(velocity.x));
                    }

                    _collisions.left = dirX == -1;
                    _collisions.right = dirX == 1;
                }
            }
        }
    }
    private void VerticleCollisions(ref Vector3 velocity)
    {
        float dirY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (dirY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticleRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * dirY, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.up * dirY * rayLength, Color.red);
            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * dirY;
                rayLength = hit.distance;

                if (_collisions.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(_collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                _collisions.below = dirY == -1;
                _collisions.above = dirY == 1;
            }
        }
        if (_collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != _collisions.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    _collisions.slopeAngle = slopeAngle;
                }
            }

        }
    }


    private void ClimbSlope(ref Vector3 velocity, float SlopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(SlopeAngle * Mathf.Deg2Rad) * moveDistance;
        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(SlopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            _collisions.below = true;
            _collisions.climbingSlope = true;
            _collisions.slopeAngle = SlopeAngle;
        }
    }

    private void DescendSlope(ref Vector3 velocity)
    {
        float dirX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (dirX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);
        if (hit)
        {

            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == dirX)
                {
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad * Mathf.Abs(velocity.x)))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        _collisions.slopeAngle = slopeAngle;
                        _collisions.descendingSlope = true;
                        _collisions.below = true;
                    }
                }
            }
        }
    }
    IEnumerator Attacking()
    {
        _attacking = true;
        _canAttack = false;
        yield return new WaitForSeconds(.5f);
        _attacking = false;
        _canAttack = true;

    }


    public void Attack()
    {
        Debug.Log("Process attack");
        _canAttack = false;
        StartCoroutine(Attacking());
        _meleeAttack.Attack();
        Debug.Log("Coroutine done");

    }

}
