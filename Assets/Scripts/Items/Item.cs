using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public static int iid;
    private int _quantity;
    public int GetQuantity { get { return _quantity; } }

    void Awake()
    {
    }

    public void SetQuantity(int quantity)
    {
        _quantity = quantity;
    }

    public void AddQuantity(int quantity)
    {
        _quantity += quantity;
    }

    public virtual int GetIID() { return iid; }

}
