using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Creatures
{

    [SerializeField] private GameObject _meleeAttackObject;
    private MeleeAttack _meleeAttack;

    void Start()
    {
        _meleeAttack = _meleeAttackObject.GetComponent<MeleeAttack>();
    }

    // Start is called before the first frame update 

    // Update is called once per frame
    void Update()
    {
        if (_body.velocity.y < -0.5f)
        {
            _body.velocity += (Vector2.up * Physics2D.gravity * (_fallMultipler - 1) * Time.deltaTime) * _body.mass;
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
            _body.AddForce(_direction * _movementSpeed * _body.mass);
            if (_direction.x > 0)
            {
                _facing = Vector2.right;

            }
            else if (_direction.x < 0)
            {
                _facing = Vector2.left;
            }
        }

        _meleeAttack.transform.localPosition = new Vector3(_facing.x * .8f, 0f, 0f);
        if (transform.position.y <= -30)
        {
            transform.position = new Vector3(-6, -2, 0);
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
    public void Move(Vector2 dir)
    {
        _direction = dir;
    }
}
