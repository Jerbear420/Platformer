using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HostileController : MonoBehaviour
{
    Vector2 _direction;

    private Hostile _hostile;

    private float deltaTime;

    void Awake()
    {
        _direction = new Vector2(1, 0);
        _hostile = GetComponent<Hostile>();
        deltaTime = 0f;
    }

    void FixedUpdate()
    {
        if (deltaTime >= 1f)
        {
            CheckHorizion();
            CheckVertical();
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
                    Debug.Log("hit player!");
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
        rayOrigin += Vector2.right * (_hostile.verticleRaySpacing + (_hostile.Velocity.x / 2));
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * dirY, rayLength, _hostile.collisionMask);
        Debug.DrawRay(rayOrigin, Vector2.up * dirY * rayLength, Color.gray);
        if (!hit)
        {
            _direction.x = -_direction.x;
        }

        if (_hostile._collisions.climbingSlope)
        {
            //  float directionX = Mathf.Sign(velocity.x);
            //  rayLength = Mathf.Abs(velocity.x) + _hostile.SkinWidth;
            // Vector2 rayOrigin = ((directionX == -1) ? _hostile.raycastOrigins.bottomLeft : _hostile.raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            // RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, _hostile.collisionMask);


        }
    }
    private float GetRandomDirection()
    {

        float x = Random.Range(1, 3) - 2;
        Debug.Log(x);
        return x;
    }

}
