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
    protected bool _attacking;
    protected Vector2 _facing = Vector2.zero; //0 = right, 1 = left, 2 = up, 3 = down 
    public Vector2 GetFacing { get { return _facing; } }
    public Vector2 SetFacing { set { _facing = value; } }
    public bool CanAttack { get { return _canAttack; } }
    protected Rigidbody2D _body;
    protected Dictionary<Creatures, float> _ignoreHit;
    public Rigidbody2D Body { get { return _body; } }
    public float MovementSpeed { get { return _movementSpeed; } }
    public float JumpPower { get { return _jumpPower; } }
    public float AttackSpeed { get { return _attackSpeed; } }
    private Health _health;
    public Health Health { get { return _health; } }
    protected bool _canJump;
    protected bool _falling;
    public bool CanJump { get { return _canJump; } }
    public bool Falling { get { return _falling; } }
    protected Vector2 _direction;
    protected CapsuleCollider2D _attackCollider;
    public Vector2 Direction { get { return _direction; } set { _direction = value; } }
    protected BoxCollider2D _hitBox;
    protected float _fallMultipler;
    // Start is called before the first frame update
    void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _falling = false;
        _canAttack = true;
        _attackCollider = gameObject.GetComponent<CapsuleCollider2D>();
        _hitBox = gameObject.GetComponent<BoxCollider2D>();
        _ignoreHit = new Dictionary<Creatures, float>();
        _canJump = true;
        _fallMultipler = 2.5f;
        _attacking = false;
        _facing = Vector2.right;
        _health = GetComponent<Health>();
        _health.RegisterDeathMethod(OnDeath);
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void OnDeath()
    {
        Debug.Log("We died.");
        Destroy(gameObject);
    }

}
