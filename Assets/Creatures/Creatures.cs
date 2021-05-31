using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Backpack))]
public abstract class Creatures : RaycastController
{

    [SerializeField] protected float _movementSpeed;
    [SerializeField] protected float _attackSpeed;
    [SerializeField] protected float _maxJumpPower;
    [SerializeField] protected bool _hostile;

    protected float minJumpPower;
    public float MinJumpPower { get { return minJumpPower; } }

    //Jump data
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    float accelerationTimeAirborn = .2f;
    float accelerationTimeGround = .1f;
    float _timeToJumpApex = .4f;
    public bool fallThrough;
    public float wallStickTime = .25f;
    float timeToWallUnstick;
    public float wallSlideSpeedMax = 3;
    protected float _maxJumpVelocity;
    protected float _minJumpVelocity;
    private bool wallSliding;

    private int wallDirX;
    private Animator _animator;
    private float velocityXSmoothing;
    protected bool _canAttack;
    protected bool _attacking;
    public bool CanAttack { get { return _canAttack; } }
    protected Rigidbody2D _body;
    protected Dictionary<Creatures, float> _ignoreHit;
    public Rigidbody2D Body { get { return _body; } }
    public float MovementSpeed { get { return _movementSpeed; } }
    public float MaxJumpPower { get { return _maxJumpPower; } }
    public float AttackSpeed { get { return _attackSpeed; } }
    private Health _health;
    public Health Health { get { return _health; } }
    protected bool _jump;
    protected Vector2 _direction;
    protected Vector2 _velocity;
    protected bool _doubleJump;
    public Vector2 Velocity { get { return _velocity; } }
    public Vector2 Direction { get { return _direction; } set { _direction = value; } }
    protected BoxCollider2D _hitBox;
    protected float _fallMultipler;
    private SpriteRenderer _renderer;
    float maxClimbSlope = 80f;
    float maxDescendAngle = 75f;
    public CollisionInfo _collisions;
    private float _gravity;
    private bool _fallThrough;
    public bool FallThrough { get { return _fallThrough; } set { _fallThrough = value; } }

    protected Backpack _backpack;
    public Backpack Backpack { get { return _backpack; } }
    private IInteractable _interactable;
    private Transform interactableCache;
    public Interactable Interactable { get { return _interactable.Interactable; } }

    protected virtual void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _backpack = GetComponent<Backpack>();
        _canAttack = true;
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _collisions = new CollisionInfo();
        _hitBox = gameObject.GetComponent<BoxCollider2D>();
        _ignoreHit = new Dictionary<Creatures, float>();
        _fallMultipler = 2.5f;
        _attacking = false;
        _health = GetComponent<Health>();
        _health.RegisterDeathMethod(OnDeath);
        _collisions.faceDir = 1;
        _gravity = -(2 * _maxJumpPower) / Mathf.Pow(_timeToJumpApex, 2);
        _maxJumpVelocity = Mathf.Abs(_gravity * _timeToJumpApex);
        _minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(_gravity) * minJumpPower);
    }

    public void Move(Vector2 velocity, bool standingOnPlatform = false)
    {

        UpdateRaycastOrigins();
        _collisions.Reset();
        _collisions.velocityOld = velocity;
        if (velocity.x != 0)
        {

            _collisions.faceDir = (int)Mathf.Sign(velocity.x);
        }
        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }
        HorizontalCollisions(ref velocity);
        if (velocity.y != 0f)
        {
            VerticleCollisions(ref velocity);
            if (velocity.y < 0f)
            {
                _animator.SetBool("Jump", false);
            }
        }
        transform.Translate(velocity);
        if (standingOnPlatform)
        {
            _collisions.below = true;
        }
    }

    void FixedUpdate()
    {

        wallSliding = false;
        wallDirX = (_collisions.left) ? -1 : 1;
        float targetVelocityX = _direction.x * _movementSpeed;
        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref velocityXSmoothing, (_collisions.below) ? accelerationTimeGround : accelerationTimeAirborn);

        HandleWallSliding();
        _velocity.y += _gravity * Time.deltaTime;
        Move(_velocity * Time.deltaTime);
        if (_collisions.above || _collisions.below)
        {
            velocityXSmoothing = 0;
            _velocity.y = 0;
            if (_collisions.below)
            {
                _doubleJump = false;
                _animator.SetBool("Jump", false);
                _animator.SetBool("Grounded", true);

            }
        }
        if (_interactable != null)
        {
            var mag = (_interactable.Interactable.transform.position - transform.position).magnitude;
            if (mag >= 2.5f)
            {
                ClearInteractables();
            }
        }
        Animate();
    }

    private void Animate()
    {
        if (_animator != null)
        {
            if (_direction.x < 0)
            {
                _renderer.flipX = true;
                _animator.SetBool("Skid", false);
            }
            else if (_direction.x > 0)
            {
                _renderer.flipX = false;
                _animator.SetBool("Skid", false);
            }
            else if (_direction.x == 0)
            {
                _animator.SetBool("Skid", true);
            }

            _animator.SetFloat("X", Mathf.Abs(_velocity.x));
            _animator.speed = 1 + (Mathf.Abs(_velocity.x) / 100);
        }
    }
    private void HandleWallSliding()
    {
        if ((_collisions.left || _collisions.right) && !_collisions.below && _velocity.y < 0)
        {
            wallSliding = true;
            if (_velocity.y < -wallSlideSpeedMax)
            {
                _velocity.y = -wallSlideSpeedMax;
            }
            if (timeToWallUnstick > 0)
            {
                _velocity.x = 0;
                if (_direction.x != wallDirX && _direction.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
    }
    public void Jump(bool released = false)
    {
        if ((_collisions.below || wallSliding || _doubleJump) && !released)
        {
            if (_doubleJump)
            {
                _velocity.y = _maxJumpVelocity;
                _animator.SetBool("Grounded", false);
                _animator.SetBool("Jump", true);

                _doubleJump = false;
            }
            else if (!wallSliding)
            {
                _velocity.y = _maxJumpVelocity;
                _animator.SetBool("Grounded", false);
                _animator.SetBool("Jump", true);
                _doubleJump = true;
            }
            else
            {

                if (wallDirX == _direction.x)
                {
                    _velocity.x = -wallDirX * wallJumpClimb.x;
                    _velocity.y = wallJumpClimb.y;
                }
                else if (_direction.x == 0)
                {
                    _velocity.x = -wallDirX * wallJumpOff.x;
                    _velocity.y = wallJumpOff.y;
                }
                else
                {
                    _velocity.x = -wallDirX * wallLeap.x;
                    _velocity.y = wallLeap.y;
                }
            }

        }
        else if (_velocity.y > _minJumpVelocity && released)
        {
            _velocity.y = _minJumpVelocity;
        }
    }
    public void Jump(float force)
    {
        _velocity.y += force;
        var hold = _maxJumpVelocity;
        _maxJumpVelocity = _velocity.y;
        Jump();
        _maxJumpVelocity = hold;

    }
    protected void OnDeath()
    {
        Debug.Log("We died.");
        Destroy(gameObject);
    }

    public void ClearInteractables()
    {
        _interactable = null;
        interactableCache = null;
    }
    private void HorizontalCollisions(ref Vector2 velocity)
    {
        float dirX = _collisions.faceDir;
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        if (Mathf.Abs(velocity.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (dirX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * dirX * rayLength, Color.blue);
            if (hit)
            {
                if (hit.collider.tag == "Interactable")
                {
                    IInteractable ib = Interactable.AllInteractors[hit.collider.transform];
                    ib.Nearby(this);
                    _interactable = ib;
                    if (ib.PassThrough)
                    {
                        continue;
                    }
                }
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
                }
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
    private void VerticleCollisions(ref Vector2 velocity)
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
                if (hit.collider.tag == "Interactable")
                {
                    IInteractable ib = Interactable.AllInteractors[hit.collider.transform];
                    if (ib != null)
                    {
                        ib.Nearby(this);
                    }
                    if (ib.PassThrough)
                    {
                        continue;
                    }
                }
                if (hit.collider.tag == "VerticalThrough")
                {
                    if (dirY == 1 || hit.distance == 0)
                    {
                        continue;
                    }
                    if (_fallThrough)
                    {
                        continue;
                    }
                }

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


    private void ClimbSlope(ref Vector2 velocity, float SlopeAngle)
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

    private void DescendSlope(ref Vector2 velocity)
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



}
