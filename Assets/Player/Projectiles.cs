using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : RaycastController
{
    protected float _fallMultipler;
    [SerializeField] private float _lifeTime;
    private float _ltDelta;
    private SpriteRenderer _renderer;
    public CollisionInfo _collisions;
    float accelerationTimeAirborn = .2f;
    float accelerationTimeGround = .1f;
    [SerializeField] private float _gravity;
    private Vector2 _velocity;
    private Vector2 _direction;
    [SerializeField] private float _movementSpeed;
    private Creatures _owner;
    private float velocityXSmoothing;
    public static Dictionary<Transform, Projectiles> LiveProjectiles = new Dictionary<Transform, Projectiles>();
    void Awake()
    {
        _ltDelta = 0f;
        _renderer = GetComponent<SpriteRenderer>();
        _collisions = new CollisionInfo();
        _fallMultipler = 2.5f;
        _collisions.faceDir = 1;
        _velocity = Vector2.zero;
        _direction = Vector2.zero;
        LiveProjectiles.Add(transform, this);
    }

    void FixedUpdate()
    {
        if (_lifeTime > 0)
        {
            if (_ltDelta >= _lifeTime)
            {
                LiveProjectiles.Remove(transform);
                Destroy(gameObject);
                return;
            }
            else
            {
                _ltDelta += Time.deltaTime;
            }

        }

        //float targetVelocityX = ;
        _velocity.x = _direction.x * _movementSpeed;
        //Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref velocityXSmoothing, (_collisions.below) ? accelerationTimeGround : accelerationTimeAirborn);

        if (_gravity != 0)
        {
            _velocity.y += _gravity * Time.deltaTime;
        }
        Move(_velocity * Time.deltaTime);
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
        HorizontalCollisions(ref velocity);

        VerticleCollisions(ref velocity);

        transform.Translate(velocity);

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
                if (Interactable.AllInteractors.ContainsKey(hit.transform))
                {
                    if (!Interactable.AllInteractors[hit.transform].PassThrough)
                    {
                        LiveProjectiles.Remove(transform);
                        Destroy(gameObject);
                    }
                    else
                    {
                        continue;
                    }
                }

                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Hostile") || hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    if (Creatures.AllCreatures.ContainsKey(hit.transform) && this._owner != Creatures.AllCreatures[hit.transform])
                    {
                        Creatures.AllCreatures[hit.transform].Damage(1, _owner);
                        LiveProjectiles.Remove(transform);
                        Destroy(gameObject);
                    }
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
                if (hit.transform.tag == "Interactable")
                {
                    if (Interactable.AllInteractors.ContainsKey(hit.transform))
                    {
                        if (!Interactable.AllInteractors[hit.transform].PassThrough)
                        {
                            LiveProjectiles.Remove(transform);
                            Destroy(gameObject);
                        }
                        else
                        {
                            continue;
                        }
                    }

                }


            }
        }
    }
    public Creatures GetOwner()
    {
        return _owner;
    }
    public void SetOwner(Creatures owner)
    {
        _owner = owner;
    }
    public void SetDirection(float dirX, Vector2 postion)
    {

        transform.position = postion + new Vector2(dirX, .33f);
        _direction = new Vector2(dirX, 0);
    }

}
