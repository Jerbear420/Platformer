using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private InputAction _movement;
    [SerializeField] private InputActionAsset _playerControls;
    private Vector2 _direction;
    void Start()
    {
        var gap = _playerControls.FindActionMap("Movement");

        _movement = gap.FindAction("Move");

        _movement.performed += OnMovementChanged;

        _movement.canceled += OnMovementChanged;
        _direction = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (_direction != Vector2.zero)
        {

        }
    }

    private void OnMovementChanged(InputAction.CallbackContext context)
    {
        var dir = context.ReadValue<Vector2>();
        _direction = new Vector2(dir.x, dir.y);
    }
}
