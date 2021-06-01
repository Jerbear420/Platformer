using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Player : Creatures
{

    public static Dictionary<Transform, Player> PlayerList = new Dictionary<Transform, Player>();

    [SerializeField] private Projectiles _projectile;
    protected override void Awake()
    {
        base.Awake();
        PlayerList.Add(transform, this);
    }


    public override void Attack()
    {
        if (lastAttackDelta == 0f || lastAttackDelta + AttackSpeed < Time.fixedTime)
        {
            lastAttackDelta = Time.fixedTime;
            if (_animator != null)
            {
                _animator.SetTrigger("Attack");
            }
            Projectiles newShot = Instantiate(_projectile.gameObject).GetComponent<Projectiles>();
            newShot.SetDirection(_collisions.faceDir, transform.position);
            Debug.Log("Attack!");
        }
        else
        {
            Debug.Log("Cooldown");
        }
    }

}
