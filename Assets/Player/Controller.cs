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
    private Player _player;
    void Start()
    {
        var gap = _playerControls.FindActionMap("Movement");
        _movement = gap.FindAction("Move");
        _attack = gap.FindAction("Attack");
        _jumpAction = gap.FindAction("Jump");
        _downAction = gap.FindAction("Down");
        _movement.performed += OnMovementChanged;
        _movement.canceled += OnMovementChanged;
        _attack.performed += ctx => OnAttack(ctx);
        _jumpAction.performed += ctx => OnJump(ctx);
        _jumpAction.canceled += ctftx => OnJump(ctftx);
        _downAction.performed += dtx => OnDown(dtx);
        _downAction.canceled += dtx => OnDown(dtx);
        _player = GetComponent<Player>();
        _direction = Vector2.zero;
    }


    private void OnMovementChanged(InputAction.CallbackContext context)
    {
        var dir = context.ReadValue<Vector2>();
        _player.Direction = new Vector2(dir.x, _player.Direction.y);

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
        if (context.performed)
        {
            _player.Jump();
        }
        else if (context.canceled)
        {
            _player.Jump(true);
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
            _player.FallThrough = false;
        }
    }
}