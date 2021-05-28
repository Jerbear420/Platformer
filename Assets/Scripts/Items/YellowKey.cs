using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowKey : Keys
{

    new public static int iid = 2;
    public override int GetIID()
    {
        Debug.Log("Inside YKey");
        return iid;
    }


}
