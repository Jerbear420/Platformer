using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenKey : Keys
{

    new public static int iid = 5;
    public override int GetIID()
    {
        Debug.Log("Inside GKey");
        return iid;
    }


}
