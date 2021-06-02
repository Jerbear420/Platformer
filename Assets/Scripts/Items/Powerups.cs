using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerups : Item
{

    [SerializeField] private float _bonusSpeed;
    public float BonusSpeed { get { return _bonusSpeed; } }
    [SerializeField] private float _lifeTime;
    public float LifeTime { get { return _lifeTime; } }
    private float _activatedTime;
    public float ActivatedTime { get { return _activatedTime; } }
    new public static int iid = 7;
    public override int GetIID()
    {
        return iid;
    }
    public override void Nearby(Creatures interactor)
    {
        if (PickupEnabled && gameObject.activeSelf)
        {
            interactor._powerupData.Powerup(this, _lifeTime);
            interactor.ClearInteractables();
            _activatedTime = Time.fixedTime;
            transform.SetParent(interactor.Backpack.gameObject.transform);
            gameObject.SetActive(false);
        }
    }

}
