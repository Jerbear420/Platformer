using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerController : PlatformerSystem
{
    // Start is called before the first frame update
    private InputAction _movement;


    private InputAction _attack;
    private InputAction _jumpAction;
    private InputAction _downAction;
    private InputAction _interactAction;
    private List<InputAction.CallbackContext> _callbackContext;
    [SerializeField] private InputActionAsset _playerControls;
    private Vector2 _direction;
    private Player _player;
    private bool _loaded;
    void Start()
    {
        _loaded = false;
        _callbackContext = new List<InputAction.CallbackContext>();
        var gap = _playerControls.FindActionMap("Movement");
        _movement = gap.FindAction("Move");
        _attack = gap.FindAction("Attack");
        _jumpAction = gap.FindAction("Jump");
        _downAction = gap.FindAction("Down");
        _interactAction = gap.FindAction("Interact");
        _player = GetComponent<Player>();
        _direction = Vector2.zero;
        Initialize();
        _loaded = true;
    }

    void Initialize()
    {
        _movement.performed += OnMovementChanged;
        _movement.canceled += OnMovementChanged;
        _attack.performed += OnAttack;
        _jumpAction.performed += OnJump;
        _jumpAction.canceled += OnJump;
        _interactAction.performed += OnInteract;
        _downAction.performed += OnDown;
        _downAction.canceled += OnDown;
    }

    void OnDisable()
    {
        if (_loaded)
        {
            _movement.performed -= OnMovementChanged;
            _movement.canceled -= OnMovementChanged;
            _attack.performed -= OnAttack;
            _jumpAction.performed -= OnJump;
            _jumpAction.canceled -= OnJump;
            _interactAction.performed -= OnInteract;
            _downAction.performed -= OnDown;
            _downAction.canceled -= OnDown;
        }
    }

    private void OnMovementChanged(InputAction.CallbackContext context)
    {
        var dir = context.ReadValue<Vector2>();
        _player.Direction = new Vector2(dir.x, _player.Direction.y);
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        _player.Attack();
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
        if (context.performed)
        {
            _player.FallThrough = true;
        }
        if (context.canceled)
        {
            _player.FallThrough = false;
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_player.Interactable != null)
            {
                if ((_player.Interactable.transform.position - _player.transform.position).magnitude <= 2f)
                {
                    _player.Interactable.Interacted(_player);
                }
                else
                {
                    _player.ClearInteractables();
                }
            }
        }
    }
}