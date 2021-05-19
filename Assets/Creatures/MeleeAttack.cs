using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : PlatformerSystem
{

    private Creatures _owner;
    private CapsuleCollider2D _hitBox;

    [SerializeField] private float _minDmg;
    [SerializeField] private float _maxDmg;
    public float MinDmg { get { return _minDmg; } }
    public float MaxDmg { get { return _maxDmg; } }

    private List<Creatures> _inRange;
    // Start is called before the first frame update
    void Awake()
    {
        _owner = transform.parent.GetComponent<Creatures>();
        _hitBox = GetComponent<CapsuleCollider2D>();
        _inRange = new List<Creatures>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Melee attack collider trigger enter");
        var creature = collider.GetComponent<Creatures>();
        if (creature)
        {
            var exists = _inRange.Contains(creature);
            if (!exists)
            {
                Debug.Log("Adding to list");
                _inRange.Add(creature);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        var creature = collider.GetComponent<Creatures>();
        if (creature)
        {
            var exists = _inRange.Contains(creature);
            if (exists)
            {
                Debug.Log("Removing from list");
                _inRange.Remove(creature);
            }
        }
    }

    public void Attack()
    {
        for (var i = _inRange.Count - 1; i >= 0; i--)
        {
            Debug.Log("Hit creature!");
            var creature = _inRange[i];
            var dmg = Random.Range(_minDmg - 1, _maxDmg + 1);
            Debug.Log(dmg);
            creature.Health.TakeDamage(dmg);
        }
    }
}
