using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : PlatformerSystem
{

    public delegate void OnDeath();
    public OnDeath _deathMethod;
    private Creatures _owner;
    [SerializeField] private float _maxHealth;
    private float _currentHealth;
    public float MaxHealth { get { return _maxHealth; } }
    public float CurrentHealth { get { return _currentHealth; } }

    public delegate void DamageReceived();
    public event DamageReceived OnHurt;
    void Awake()
    {
        _owner = GetComponent<Creatures>();
        _currentHealth = _maxHealth;
    }

    public void RegisterDeathMethod(OnDeath method)
    {
        _deathMethod = method;
    }

    public void Heal(float _health)
    {
        _currentHealth = Mathf.Min(_currentHealth + _health, _maxHealth);
        if (OnHurt != null)
        {
            OnHurt();
        }
    }

    public void TakeDamage(float damage, Creatures attacker)
    {
        if (_currentHealth - damage <= 0f)
        {
            _currentHealth = 0f;
            if (attacker is Player)
            {
                Player p = attacker as Player;
                p.SaveData.AddScore(_owner.ScoreBonus);
            }
            _deathMethod();

        }
        else
        {
            _currentHealth -= damage;
        }
        if (OnHurt != null)
        {
            OnHurt();
        }
    }
}
