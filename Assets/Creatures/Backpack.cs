using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    private Creatures owner;

    private Dictionary<int, Item> _storage;
    public Dictionary<int, Item> Storage { get { return _storage; } }
    void Awake()
    {
        _storage = new Dictionary<int, Item>();
    }

    public void AddItem(Item item)
    {
        if (_storage.ContainsKey(item.GetIID()))
        {
            Item existing = _storage[item.GetIID()];
            existing.AddQuantity(item.GetQuantity);
            Destroy(item);
        }
        else
        {
            _storage.Add(item.GetIID(), item);
            item.gameObject.SetActive(false);
            item.transform.parent = this.gameObject.transform;
        }
    }

    public bool ContainsItem(int itemID)
    {
        Debug.Log(_storage.ContainsKey(itemID));
        return _storage.ContainsKey(itemID);
    }


    public void RemoveItem(int itemID)
    {
        _storage.Remove(itemID);
    }
}
