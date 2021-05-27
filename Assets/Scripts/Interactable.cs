using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    public delegate void OnInteract(Creatures interactor);
    OnInteract interaction;

    public void RegisterInteraction(OnInteract i)
    {
        interaction = i;
        Debug.Log("Registered interaction");
    }

    public void Interacted(Creatures interator)
    {
        interaction(interator);
    }
}
