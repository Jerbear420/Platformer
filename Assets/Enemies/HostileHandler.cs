using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostileHandler : Creatures
{
    private Creatures _target;
    private int _status; //1 = wander, 2 = follow & attack
    // Start is called before the first frame update
    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player").GetComponent<Creatures>();
        _status = 1;
        _facing = Vector2.right;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (_target != null)
        {
            if (_target.transform.position.y - transform.position.y <= 2)
            {
                _body.AddForce((_target.transform.position - transform.position).normalized * _body.mass * _movementSpeed);
                Debug.Log("A");
            }
            else
            {
                Debug.Log("B");
                _status = 1;
                Debug.Log(_facing);
                var ray = Physics2D.Raycast(transform.position, (new Vector3(transform.position.x + (_facing * 4).x, transform.position.y - 2, 0)), 3f);
                Debug.DrawRay(transform.position, (new Vector3(transform.position.x + (_facing * 4).x, transform.position.y - 2, 0)), Color.red, 3f);
                if (ray.collider)
                {
                    Debug.Log("C");
                    _body.AddForce(_facing * _body.mass * _movementSpeed);
                }
                else
                {
                    Debug.Log("D");
                    _body.velocity = Vector2.zero;
                    _facing = -_facing;
                }
            }
        }
        else
        {
            Debug.Log("No target found");
            _target = GameObject.FindGameObjectWithTag("Player").GetComponent<Creatures>();
        }
    }
}
