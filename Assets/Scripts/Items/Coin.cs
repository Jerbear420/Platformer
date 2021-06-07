using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Item
{
    new public static int iid = 1;
    public override int GetIID()
    {
        return iid;
    }
}
