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
    private InputAction _jumpAction;
    private InputAction _downAction;
    [SerializeField] private InputActionAsset _playerControls;
    private Vector2 _direction;
    private Vector3 _velocity;
    private Player _player;
    private Vector2 jumpVelocity;
    float accelerationTimeAirborn = .2f;
    float accelerationTimeGround = .1f;
    float _timeToJumpApex = .4f;
    private bool jump;
    public bool fallThrough;
    public float wallSlideSpeedMax = 3;
    protected float _maxJumpVelocity;
    protected float _minJumpVelocity;
    private float _gravity;
    private float velocityXSmoothing;
    public float wallStickTime = .25f;
    float timeToWallUnstick;
    void Start()
    {
        var gap = _playerControls.FindActionMap("Movement");
        _movement = gap.FindAction("Move");
        _attack = gap.FindAction("Attack");
        _jumpAction = gap.FindAction("Jump");
        _downAction = gap.FindAction("Down");
        _player = GetComponent<Player>();
        _movement.performed += OnMovementChanged;
        _attack.performed += ctx => OnAttack(ctx);
        _jumpAction.performed += cttx => OnJump(cttx);
        _downAction.performed += dtx => OnDown(dtx);
        jump = false;
        _gravity = -(2 * _player.MaxJumpPower) / Mathf.Pow(_timeToJumpApex, 2);
        _maxJumpVelocity = Mathf.Abs(_gravity * _timeToJumpApex);
        _minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(_gravity) * _player.minJumpPower);
        Debug.Log("Gravity" + _gravity);
        Debug.Log("JumpVelocity" + _maxJumpVelocity);
        _movement.canceled += OnMovementChanged;
        _jumpAction.canceled += ctftx => OnJump(ctftx);
        _downAction.canceled += dtx => OnDown(dtx);
        _direction = Vector2.zero;
        _velocity = Vector2.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        bool wallSliding = false;
        int wallDirX = (_player._collisions.left) ? -1 : 1;
        float targetVelocityX = _direction.x * _player.MovementSpeed;
        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref velocityXSmoothing, (_player._collisions.below) ? accelerationTimeGround : accelerationTimeAirborn);

        if ((_player._collisions.left || _player._collisions.right) && !_player._collisions.below && _velocity.y < 0)
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


        if (jump)
        {
            if (wallSliding)
            {
                Debug.Log("Sliding!");
                if (wallDirX == _direction.x)
                {
                    _velocity.x = -wallDirX * _player.wallJumpClimb.x;
                    _velocity.y = _player.wallJumpClimb.y;
                    jump = false;
                }
                else if (_direction.x == 0)
                {
                    _velocity.x = -wallDirX * _player.wallJumpOff.x;
                    _velocity.y = _player.wallJumpOff.y;
                    jump = false;
                }
                else
                {
                    _velocity.x = -wallDirX * _player.wallLeap.x;
                    _velocity.y = _player.wallLeap.y;
                    jump = false;
                }
            }
            if (_player._collisions.below)
            {
                _velocity.y = _maxJumpVelocity;
                jump = false;
            }
        }
        _velocity.y += _gravity * Time.deltaTime;
        _player.Move(_velocity * Time.deltaTime);
        if (_player._collisions.above || _player._collisions.below)
        {
            velocityXSmoothing = 0;
            _velocity.y = 0;
        }
    }

    private void OnMovementChanged(InputAction.CallbackContext context)
    {
        var dir = context.ReadValue<Vector2>();
        _direction = dir;

        if (dir.y > 0f)
        {
            Debug.Log("Jump!");
            //jump = true;
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
    private void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump called");
        if (context.performed)
        {
            if (!jump)
            {
                jump = true;
            }
        }
        if (context.canceled)
        {
            Debug.Log("Released jump");
            if (_velocity.y > _minJumpVelocity)
            {
                Debug.Log("Velocity is higher than jump velocity");
                _velocity.y = _minJumpVelocity;
            }
        }
    }
    private void OnDown(InputAction.CallbackContext context)
    {
        Debug.Log("down called");
        if (context.performed)
        {
            _player.FallThrough = true;
        }
        if (context.canceled)
        {
            Debug.Log("Released down");
            _player.FallThrough = false;
        }
    }
}