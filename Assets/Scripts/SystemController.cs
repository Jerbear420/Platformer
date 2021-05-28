using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteractable
{
    Interactable Interactable { get; }
    bool PassThrough { get; }
    void Interact(Creatures interactor);
    void Nearby(Creatures interactor);
}
interface ITriggerable
{
    void OnTrigger();
    bool Triggered { get; }
    bool CanTrigger { get; }
}
public class SystemController : MonoBehaviour
{
    [SerializeField]
    public List<Item> ItemToLoad = new List<Item>();
    public static Dictionary<int, Item> ItemList = new Dictionary<int, Item>();
    // Start is called before the first frame update
    void Awake()
    {
        foreach (var item in ItemToLoad)
        {
            if (!ItemList.ContainsKey(item.GetIID()))
            {
                ItemList.Add(item.GetIID(), item);
                Debug.Log(item.GetIID() + " Was loaded");
            }
        }

        Debug.Log("Loaded items");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
