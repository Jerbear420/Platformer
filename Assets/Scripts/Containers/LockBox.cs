using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockBox : Containers
{
    [SerializeField] private Item key;

    [SerializeField] private Item Reward;
    [SerializeField] private Sprite disabledImage;
    [SerializeField] private Sprite enabledImage;
    public override void Interact(Creatures interactor)
    {

        if (interactor.Backpack.ContainsItem(key.GetIID()))
        {
            Item i = interactor.Backpack.Storage[key.GetIID()];
            i.OnUse();
            Debug.Log("Reward needed!");
            _renderer.sprite = disabledImage;
        }

    }
}
