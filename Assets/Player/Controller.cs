using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class Controller : PlatformerSystem
{
    // Start is called before the first frame update
    private InputAction _movement;

    private InputAction _attack;
    [SerializeField] private InputActionAsset _playerControls;
    private Vector2 _direction;
    private Vector3 _velocity;
    private Player _player;
    private Vector2 jumpVelocity;
    float accelerationTimeAirborn = .2f;
    float accelerationTimeGround = .1f;
    float _timeToJumpApex = .4f;
    private bool jump;

    protected float _jumpVelocity;
    private float _gravity;
    private float velocityXSmoothing;
    void Start()
    {
        var gap = _playerControls.FindActionMap("Movement");
        _movement = gap.FindAction("Move");
        _attack = gap.FindAction("Attack");
        _player = GetComponent<Player>();
        _movement.performed += OnMovementChanged;
        _attack.performed += ctx => OnAttack(ctx);
        jump = false;
        _gravity = -(2 * _player.JumpPower) / Mathf.Pow(_timeToJumpApex, 2);
        _jumpVelocity = Mathf.Abs(_gravity * _timeToJumpApex);
        Debug.Log("Gravity" + _gravity);
        Debug.Log("JumpVelocity" + _jumpVelocity);
        _movement.canceled += OnMovementChanged;
        _direction = Vector2.zero;
        _velocity = Vector2.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (_player._collisions.above || _player._collisions.below)
        {
            _velocity.y = 0;
        }
        if (jump)
        {
            _velocity.y = _jumpVelocity;
            jump = false;
        }
        float targetVelocityX = _direction.x * _player.MovementSpeed;
        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref velocityXSmoothing, (_player._collisions.below) ? accelerationTimeGround : accelerationTimeAirborn);
        _velocity.y += _gravity * Time.deltaTime;
        _player.Move(_velocity * Time.deltaTime);
    }

    private void OnMovementChanged(InputAction.CallbackContext context)
    {
        var dir = context.ReadValue<Vector2>();
        _direction = dir;

        if (_player._collisions.below && dir.y > 0f)
        {
            Debug.Log("Jump!");
            jump = true;
        }

        /*  if (!_player.Falling && _player.CanJump && dir.y > 0f)
          {
              _player.Jump();
          }
          if (context.canceled && _player.Body.velocity.x != 0)
          {
              {
                  _player.StopMoving();

              }
          }
  */
    }

    private void OnAttack(InputAction.CallbackContext context)
    {

        if (_player.CanAttack)
        {
            _player.Attack();
        }
        else
        {
            Debug.Log("Cant attack yet!");
        }

    }

}