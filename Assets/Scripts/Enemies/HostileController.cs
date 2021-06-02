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
            }
        }
        if (deltaTime >= deltaDelay)
        {
            if (_player != null)
            {
                var dirX = _player.transform.position.x - transform.position.x;
                _direction = new Vector2(Mathf.Sign(dirX), _direction.y);
                TryAttacking();
            }
            else
            {
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

    private void CheckHorizion()
    {
        float dirX = (_direction.x == 0) ? _hostile._collisions.faceDir : Mathf.Sign(_direction.x);
        float rayLength = 2 + _hostile.SkinWidth;

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
                    deltaDelay = .5f;
                    Debug.Log("ray player!");
                    _player = Creatures.AllCreatures[hit.transform] as Player;
                    Debug.Log(hit.distance);
                    if (hit.distance <= _attackRange)
                    {
                        Debug.Log("hit player!");
                        _hostile.Attack(_player);
                    }
                }
                else
                {
                    Debug.Log("collision ----" + hit.transform.name);
                    _direction.x = -_direction.x;

                    Debug.Log(_direction.x);
                    break;

                }

            }

        }
    }
    private void CheckVertical()
    {
        float dirY = -1;
        float rayLength = 2 + _hostile.SkinWidth;
        Vector2 rayOrigin = (_direction.x == -1) ? _hostile.raycastOrigins.bottomLeft : _hostile.raycastOrigins.bottomRight;
        rayOrigin += Vector2.right * (_hostile.verticleRaySpacing + (_hostile.Velocity.x));
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * dirY, rayLength, _hostile.collisionMask);
        Debug.DrawRay(rayOrigin, Vector2.up * dirY * rayLength, Color.gray);
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
        if ((_player.transform.position - transform.position).magnitude <= _attackRange)
        {
            _hostile.Attack(_player);
        }
    }

}
