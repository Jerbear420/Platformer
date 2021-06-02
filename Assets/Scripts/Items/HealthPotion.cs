using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : Item
{

    [SerializeField] private float _health;

    public override void Nearby(Creatures interactor)
    {
        if (PickupEnabled && gameObject.activeSelf)
        {
            if (interactor.Health.CurrentHealth < interactor.Health.MaxHealth)
            {
                interactor.Health.Heal(_health);
                interactor.ClearInteractables();
                Destroy(gameObject);
            }
        }
    }
}
