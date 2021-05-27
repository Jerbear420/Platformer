using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public abstract class Containers : MonoBehaviour
{
    [Range(1, 100)]
    [SerializeField] private int coins;

    private Interactable _interactable;

    void Awake()
    {
        _interactable = GetComponent<Interactable>();
        _interactable.RegisterInteraction(Interact);
    }
    public virtual void Interact(Creatures interactor)
    {
    }

}
