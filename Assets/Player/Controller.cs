using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : PlatformerSystem
{
    // Start is called before the first frame update
    private InputAction _movement;

    private InputAction _attack;
    [SerializeField] private InputActionAsset _playerControls;
    private Vector2 _direction;
    private Player _player;
    void Start()
    {
        var gap = _playerControls.FindActionMap("Movement");
        _movement = gap.FindAction("Move");
        _attack = gap.FindAction("Attack");
        _player = GetComponent<Player>();
        _movement.performed += OnMovementChanged;
        _attack.performed += ctx => OnAttack(ctx);

        _movement.canceled += OnMovementChanged;
        _direction = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMovementChanged(InputAction.CallbackContext context)
    {
        var dir = context.ReadValue<Vector2>();
        _player.Move(new Vector2(dir.x, 0));

        Debug.Log("AA");
        Debug.Log(_player.Falling + " ---- " + _player.CanJump);
        if (!_player.Falling && _player.CanJump && dir.y > 0f)
        {
            Debug.Log("A");
            _player.Jump();
        }
        if (context.canceled && _player.Body.velocity.x != 0)
        {
            {
                _player.StopMoving();

            }
        }

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