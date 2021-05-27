using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBox : Containers
{
    [Range(1, 100)]
    private int _coins;
    public override void Interact(Creatures interactor)
    {
        if (interactor is Player)
        {
            Player plr = interactor as Player;
            Item reward = Instantiate(SystemController.ItemList[1]);
            reward.SetQuantity(_coins);
            plr.Reward(reward);
        }
    }

}
