using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keys : Item
{

    public override void OnUse()
    {
        RemoveQuantity(1);
    }
}
