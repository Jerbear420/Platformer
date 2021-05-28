using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBox : Containers
{
    [Range(1, 100)]
    private int _coins;
    public override void Interact(Creatures interactor)
    {
        Debug.Log("Interact with coinbox");
        Item reward = Instantiate(SystemController.ItemList[1]);
        reward.SetQuantity(_coins);
        reward.SetOwner(interactor);
        interactor.Backpack.AddItem(reward);

    }

}
