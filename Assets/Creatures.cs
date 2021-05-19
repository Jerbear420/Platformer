using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Creatures : PlatformerSystem
{

    [SerializeField] protected float _movementSpeed;
    [SerializeField] protected float _attackSpeed;
    [SerializeField] protected float _jumpPower;
    [SerializeField] protected bool _hostile;
    protected bool _canAttack;
    protected float _deltaAttack;
    protected Vector2 _facing = Vector2.zero; //0 = right, 1 = left, 2 = up, 3 = down 
    public Vector2 GetFacing { get { return _facing; } }
    public Vector2 SetFacing { set { _facing = value; } }
    public bool CanAttack { get { return _canAttack; } }
    protected Rigidbody2D _body;
    public Rigidbody2D Body { get { return _body; } }
    public float MovementSpeed { get { return _movementSpeed; } }
    public float JumpPower { get { return _jumpPower; } }
    public float AttackSpeed { get { return _attackSpeed; } }
    protected bool _canJump;
    protected bool _falling;
    public bool CanJump { get { return _canJump; } }
    public bool Falling { get { return _falling; } }
    protected Vector2 _direction;
    public Vector2 Direction { get { return _direction; } set { _direction = value; } }
    protected float _fallMultipler;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
