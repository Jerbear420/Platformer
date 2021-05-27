using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Creatures
{
    private Dictionary<int, Item> _backpack;

    protected override void Awake()
    {
        base.Awake();
        _backpack = new Dictionary<int, Item>();
    }

    public void Reward(Item prize)
    {
        Debug.Log("Prize received");
        if (_backpack.ContainsKey(prize.GetIID()))
        {
            Debug.Log("Already in backpack, incrememnt");
            Item existing = _backpack[prize.GetIID()];
            existing.AddQuantity(prize.GetQuantity);
            Destroy(prize);
        }
        else
        {
            _backpack.Add(prize.GetIID(), prize);
            prize.gameObject.SetActive(false);
            Debug.Log("Added to backpack");
            prize.transform.parent = this.gameObject.transform;
        }
        Debug.Log("Prize system done");
    }


}
