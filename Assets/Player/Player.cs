using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Creatures
{



    // Start is called before the first frame update
    void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _falling = false;
        _canAttack = true;
        _canJump = true;
        _deltaAttack = 0f;
        _fallMultipler = 2.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (_body.velocity.y < -0.5f)
        {
            _body.velocity += Vector2.up * Physics2D.gravity * (_fallMultipler - 1) * Time.deltaTime;
            _falling = true;
            _canJump = false;
            Debug.Log("Now falling");
        }

        if (_falling && _body.velocity.y == 0)
        {
            _falling = false;
            _canJump = true;
            Debug.Log("Now can jump");
        }
        if (_direction != Vector2.zero)
        {
            _body.AddForce(_direction * _movementSpeed);
            if (_direction.x > 0)
            {
                _facing = Vector2.right;
            }
            else if (_direction.x < 0)
            {
                _facing = Vector2.left;
            }
        }
        if (transform.position.y <= -30)
        {
            transform.position = new Vector3(-6, -2, 0);
        }
        if (!_canAttack)
        {
            _deltaAttack += Time.deltaTime;
            if (_deltaAttack >= _attackSpeed)
            {
                _canAttack = true;
                _deltaAttack = 0f;
            }
        }
    }

    public void Jump()
    {
        Debug.Log("B");
        var _jump = Mathf.Abs(1 * (_jumpPower * Physics2D.gravity.y));
        _body.AddForce(new Vector2(0, _jump));
        _canJump = false;
        Debug.Log(_jump);
    }
    public void StopMoving()
    {
        _body.velocity = new Vector2(_body.velocity.x / 2, _body.velocity.y);
    }

    public void Attack()
    {
        GameObject _processAttack = new GameObject();
        _processAttack.transform.parent = gameObject.transform;
        _processAttack.transform.localPosition = _facing * 2f;
        Debug.Log("Process attack");
        _canAttack = false;
        _deltaAttack = 0f;
    }
    public void Move(Vector2 dir)
    {
        _direction = dir;
    }
}
