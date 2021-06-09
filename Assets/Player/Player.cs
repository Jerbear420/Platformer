using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(SaveData))]
public class Player : Creatures
{

    public static Dictionary<Transform, Player> PlayerList = new Dictionary<Transform, Player>();

    [SerializeField] private Projectiles _projectile;
    private SaveData _saveData;
    public SaveData SaveData { get { return _saveData; } }
    public override void Start()
    {
        base.Start();
        _saveData = GetComponent<SaveData>();
        PlayerList.Add(transform, this);
        transform.position = SystemController.GetSpawnPoint();
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
            newShot.SetOwner(this);
            newShot.SetDirection(_collisions.faceDir, transform.position);
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


    protected override void OnDeath()
    {

        Debug.Log("We died.");
        if (Creatures.AllCreatures.ContainsKey(transform))
        {
            Creatures.AllCreatures.Remove(transform);
        }
        SystemController.LoadScene("Overworld");
    }

    public void Save()
    {

        _saveData.writeFile();
    }


}
