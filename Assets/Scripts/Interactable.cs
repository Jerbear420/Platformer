using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    public delegate void OnInteract(Creatures interactor);
    OnInteract interaction;
    OnInteract nearby;

    public void RegisterInteraction(OnInteract i, OnInteract n)
    {
        interaction = i;
        nearby = n;
        Debug.Log("Registered interaction");
    }

    public void Interacted(Creatures interator)
    {
        interaction(interator);
    }

    public void Nearby(Creatures interator)
    {
        nearby(interator);
    }
}
