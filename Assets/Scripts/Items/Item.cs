using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class Item : MonoBehaviour, IInteractable
{
    //Powerups -- 7
    public static int iid;
    private int _quantity;
    private Creatures _owner;
    [SerializeField]
    private bool _passThrough;
    public bool PassThrough { get { return _passThrough; } }
    public int GetQuantity { get { return _quantity; } }

    [SerializeField]
    private bool _pickupEnabled;
    public bool PickupEnabled { get { return _pickupEnabled; } }
    private Interactable _interactable;
    public Interactable Interactable { get { return _interactable; } }

    protected virtual void Awake()
    {

        _interactable = GetComponent<Interactable>();
        _interactable.RegisterInteraction(Interact, Nearby);
        if (!SystemController.ItemList.ContainsKey(this.GetIID()))
        {
            SystemController.ItemList.Add(this.GetIID(), this);
        }
    }

    public void SetQuantity(int quantity)
    {
        _quantity = quantity;
    }

    public void AddQuantity(int quantity)
    {
        _quantity += quantity;
    }
    public void RemoveQuantity(int quantity)
    {
        _quantity -= quantity;
        if (_quantity <= 0)
        {
            if (_owner)
            {
                _owner.Backpack.RemoveItem(iid);
            }
            Destroy(this);
        }
    }

    public virtual void OnUse()
    {

    }

    public virtual int GetIID() { return iid; }

    public virtual void Interact(Creatures interactor)
    {
    }
    public virtual void Nearby(Creatures interactor)
    {
        Debug.Log("Player is near item");
        Debug.Log(PickupEnabled + " --------" + gameObject.activeSelf);
        if (PickupEnabled && gameObject.activeSelf)
        {
            Debug.Log("Pickup enabled, game object active, proceed to add item!");
            interactor.Backpack.AddItem(this);
        }
    }

    public Creatures GetOwner()
    {
        return _owner;
    }
    public void SetOwner(Creatures owner)
    {
        _owner = owner;
    }
}

