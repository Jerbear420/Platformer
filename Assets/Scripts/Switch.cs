using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Interactable))]
public class Switch : MonoBehaviour, IInteractable
{
    [SerializeField] private bool _passThrough;
    public bool PassThrough { get { return _passThrough; } }

    private Interactable _interactable;
    public Interactable Interactable { get { return _interactable; } }
    private SpriteRenderer _renderer;

    [SerializeField] private bool _timed;
    [SerializeField] private float _duration;
    [SerializeField] private GameObject _triggerObject;
    private ITriggerable _triggerable;
    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _interactable = GetComponent<Interactable>();
        _interactable.RegisterInteraction(Interact, Nearby);
        if (_triggerObject)
        {
            _triggerable = _triggerObject.GetComponent<ITriggerable>();
        }
    }

    public void Interact(Creatures interactor)
    {
        if (_triggerable != null)
        {
            Trigger();
            if (_timed)
            {
                Invoke("Trigger", _duration);
            }
        }
    }

    private void Trigger()
    {

        _renderer.flipX = !_renderer.flipX;
        _triggerable.OnTrigger();
    }
    public void Nearby(Creatures interactor)
    {

    }
}
