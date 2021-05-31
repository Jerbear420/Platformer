using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueKey : Keys
{

    new public static int iid = 3;
    public override int GetIID()
    {
        Debug.Log("Inside BKey");
        return iid;
    }


}
