using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Player : Creatures
{

    public static Dictionary<Transform, Player> PlayerList = new Dictionary<Transform, Player>();
    private bool HealthRegistered;

    [SerializeField] private Projectiles _projectile;
    public override void Start()
    {
        base.Start();
        PlayerList.Add(transform, this);
        transform.position = SystemController.GetSpawnPoint();
        Debug.Log("Move player");
        if (!HealthRegistered)
        {
            OnEnable();
        }
        SystemController.Controller.StartMap();
    }

    public override void Attack()
    {
        if (lastAttackDelta == 0f || lastAttackDelta + AttackSpeed / _powerupData.bonusAS < Time.fixedTime)
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

    void Update()
    {
        if (transform.position.y <= SystemController.GetYDeadZone().y)
        {
            transform.position = SystemController.GetSpawnPoint();
            _powerupData.Reset();
            Damage(1);
        }
    }

    void OnEnable()
    {
        if (Health != null)
        {
            Health.OnHurt += Hurt;
            HealthRegistered = true;
        }
        else
        {
            HealthRegistered = false;
        }
    }

    void OnDisable()
    {
        if (HealthRegistered)
        {
            Health.OnHurt -= Hurt;
        }
    }

    private void Hurt()
    {
        if (Health.CurrentHealth <= 0)
        {
            SystemController.LoadScene("Overworld");
        }
    }



}
