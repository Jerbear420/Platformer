using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(HostileController))]
public class Hostile : Creatures
{
    [SerializeField] private float _meleeDamage;
    [SerializeField] private bool _ranged;

    [SerializeField] private Projectiles _projectile;
    public override void Attack(Creatures target)
    {
        if (lastAttackDelta == 0f || lastAttackDelta + AttackSpeed < Time.fixedTime)
        {
            if (!_ranged)
            {
                target.Damage(_meleeDamage);
            }
            else
            {
                Projectiles newShot = Instantiate(_projectile.gameObject).GetComponent<Projectiles>();
                newShot.SetOwner(this);
                newShot.SetDirection(_collisions.faceDir, transform.position);
            }
            lastAttackDelta = Time.fixedTime;
        }
    }

}
