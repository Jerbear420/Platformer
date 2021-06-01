using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(HostileController))]
public class Hostile : Creatures
{
    [SerializeField] private float _attackDamage;
    public override void Attack(Creatures target)
    {
        if (lastAttackDelta == 0f || lastAttackDelta + AttackSpeed < Time.fixedTime)
        {
            Debug.Log("Do damage!");
            target.Damage(_attackDamage);
            lastAttackDelta = Time.fixedTime;
        }
    }

}
