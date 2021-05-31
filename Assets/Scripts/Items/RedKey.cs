using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedKey : Keys
{

    new public static int iid = 4;
    public override int GetIID()
    {
        Debug.Log("Inside RKey");
        return iid;
    }


}
