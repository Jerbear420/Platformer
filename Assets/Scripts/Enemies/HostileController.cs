using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HostileController : MonoBehaviour
{
    Vector2 _direction;

    private Hostile _hostile;
    [SerializeField] private float _agroRange;
    [SerializeField] private float _attackRange;
    [SerializeField] private bool _flipped;
    [SerializeField] private float _sight;
    [SerializeField] private float _hearing;
    [SerializeField]
    private LayerMask HearingMask;
    private float deltaTime;
    private float deltaDelay = 1f;
    private Player _player;
    void Awake()
    {
        _direction = (_flipped) ? -Vector2.right : Vector2.right;
        _hostile = GetComponent<Hostile>();
        deltaTime = 0f;
    }

    void Update()
    {

        if (_player != null)
        {
            if ((_player.transform.position - transform.position).magnitude > _agroRange)
            {
                deltaDelay = 1f;
                _player = null;
                _hostile._sprinting = false;
            }

        }
        if (deltaTime >= deltaDelay)
        {
            if (_player != null)
            {
                var dirX = _player.transform.position.x - transform.position.x;
                _direction = new Vector2(Mathf.Sign(dirX), _direction.y);
                CheckHorizion();
                TryAttacking();
            }
            else
            {
                CheckHearing();
                CheckHorizion();
                CheckVertical();

            }
            _hostile.Direction = _direction;
            deltaTime = 0f;
        }
        else
        {
            deltaTime += Time.deltaTime;
        }
    }

    private void CheckHearing()
    {

        float rayLength = _hearing + _hostile.SkinWidth;
        Vector2 rayOrigin = (Vector2)_hostile.transform.position + ((_hostile._collisions.faceDir == 1) ? new Vector2(_hostile._collider.bounds.extents.x, 0) : new Vector2(-_hostile._collider.bounds.extents.x, 0));
        RaycastHit2D hit = Physics2D.CircleCast(rayOrigin, rayLength + _hostile._collider.bounds.size.x, new Vector2(_hostile._collisions.faceDir, 0), 1f, HearingMask);
        if (hit)
        {
            if (hit.transform.tag == "Player")
            {
                Debug.Log("I hear a player!");
                _player = Creatures.AllCreatures[hit.transform] as Player;
                _hostile._sprinting = true;
                deltaDelay = .5f;
                deltaTime += 1f;
            }
        }
    }

    private void CheckHorizion()
    {
        float dirX = (_direction.x == 0) ? _hostile._collisions.faceDir : Mathf.Sign(_direction.x);
        float rayLength = _sight + _hostile.SkinWidth;

        for (int i = 0; i < _hostile.horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (dirX == -1) ? _hostile.raycastOrigins.bottomLeft : _hostile.raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (_hostile.horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, _hostile.collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * dirX * rayLength, Color.yellow);
            if (hit)
            {
                if (hit.transform.tag == "Player")
                {
                    if (_player == null)
                    {
                        deltaDelay = .5f;
                        _player = Creatures.AllCreatures[hit.transform] as Player;
                        _hostile._sprinting = true;
                        Debug.Log(hit.distance);
                        if (hit.distance <= _attackRange)
                        {
                            deltaTime += 1f;
                            _hostile.Attack(_player);
                        }

                    }
                }
                else if (hit.transform.tag == "Interactable")
                {
                    if (Interactable.AllInteractors.ContainsKey(hit.transform) && !Interactable.AllInteractors[hit.transform].PassThrough)
                        if (hit.distance <= .5f + _hostile.SkinWidth)
                        {
                            _direction.x = -_direction.x;
                            break;
                        }
                }
                else if (hit.transform.tag == "Projectile")
                {
                    Debug.Log("Hey a projectile!");
                    if (Projectiles.LiveProjectiles.ContainsKey(hit.transform) && Projectiles.LiveProjectiles[hit.transform].GetOwner() != _hostile)
                        if (_hostile._collisions.below)
                        {
                            Debug.Log("Jump!");
                            _hostile.Jump();
                        }
                }
                else
                {
                    var blocked = (_hostile._collisions.faceDir == 1) ? _hostile._collisions.right : _hostile._collisions.left;
                    if (blocked)
                    {
                        _direction.x = -_direction.x;

                        Debug.Log(_direction.x);
                        break;
                    }
                }

            }

        }
    }
    private void CheckVertical()
    {
        float dirY = -1;
        float rayLength = (_hostile._collisions.descendingSlope) ? 6 + _hostile.SkinWidth : 2 + _hostile.SkinWidth;
        Vector2 rayOrigin = (_direction.x == -1) ? _hostile.raycastOrigins.bottomLeft : _hostile.raycastOrigins.bottomRight;
        rayOrigin += Vector2.right * (_hostile.verticleRaySpacing + (_hostile.Velocity.x));
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * dirY, rayLength, _hostile.collisionMask);
        Debug.DrawRay(rayOrigin, Vector2.up * dirY * rayLength, Color.gray, .5f);
        if (!hit)
        {
            _direction.x = -_direction.x;
        }


    }
    private float GetRandomDirection()
    {

        float x = Random.Range(1, 3) - 2;
        Debug.Log(x);
        return x;
    }

    private void TryAttacking()
    {
        var normal = (_player.transform.position - transform.position).normalized;
        var mag = (_player.transform.position - transform.position).magnitude;
        if (mag <= _attackRange)
        {
            _hostile.Attack(_player);
            if (Mathf.Sign(normal.x) != Mathf.Sign(_hostile._collisions.faceDir))
            {
                Debug.Log("Not facing same dir?");
                Debug.Log(Mathf.Sign(normal.x));
                Debug.Log(Mathf.Sign(_hostile._collisions.faceDir));
                _direction.x = normal.x;
                return;
            }
            if (mag <= _attackRange / 2)
            {
                _direction = Vector2.zero;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (_player == null)
            {
                float rayLength = _hearing + _hostile.SkinWidth;
                Vector2 rayOrigin = (Vector2)_hostile.transform.position + ((_hostile._collisions.faceDir == 1) ? new Vector2(_hostile._collider.bounds.extents.x, 0) : new Vector2(-_hostile._collider.bounds.extents.x, 0));
                Gizmos.DrawWireSphere(rayOrigin, rayLength);
            }
        }
        else
        {
            float rayLength = _hearing;
            Vector2 rayOrigin = transform.position;
            Gizmos.DrawWireSphere(rayOrigin, rayLength);
        }
    }

}
