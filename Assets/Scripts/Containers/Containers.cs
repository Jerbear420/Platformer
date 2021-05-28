using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class Containers : MonoBehaviour, IInteractable
{
    [SerializeField] private bool _passThrough;
    public bool PassThrough { get { return _passThrough; } }
    private Interactable _interactable;
    public Interactable Interactable { get { return _interactable; } }
    protected SpriteRenderer _renderer;

    private BoxCollider2D _collider;

    protected virtual void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
        _collider.isTrigger = true;
        _interactable = GetComponent<Interactable>();
        _interactable.RegisterInteraction(Interact, Nearby);
    }
    public virtual void Interact(Creatures interactor)
    {
    }
    public virtual void Nearby(Creatures interactor)
    {
    }

}
