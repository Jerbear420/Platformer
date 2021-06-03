using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Interactable))]
public class Doorway : MonoBehaviour, IInteractable, ITriggerable
{

    private bool _canTrigger;
    public bool CanTrigger { get { return _canTrigger; } }

    private bool _triggered;
    public bool Triggered { get { return _triggered; } }
    [SerializeField] private bool _passThrough;
    public bool PassThrough { get { return _passThrough; } }

    private Interactable _interactable;
    public Interactable Interactable { get { return _interactable; } }

    [SerializeField] private bool _locked;
    [SerializeField] private Sprite _lockedSpriteBottom;
    [SerializeField] private Sprite _lockedSpriteTop;
    [SerializeField] private Sprite _unlockedSpriteBottom;
    [SerializeField] private Sprite _unlockedSpriteTop;
    [SerializeField] private GameObject _doorTop;
    [SerializeField] private Doorway _destination;
    [SerializeField] private string _teleportScene;
    private SpriteRenderer _rendererBot;
    private SpriteRenderer _rendererTop;

    void Awake()
    {
        _rendererBot = GetComponent<SpriteRenderer>();
        _rendererTop = _doorTop.GetComponent<SpriteRenderer>();
        _interactable = GetComponent<Interactable>();
        _interactable.RegisterInteraction(Interact, Nearby);
        if (!_locked)
        {

            _rendererBot.sprite = _unlockedSpriteBottom;
            _rendererTop.sprite = _unlockedSpriteTop;
        }
    }
    public void Nearby(Creatures interactor)
    {

    }
    public void Interact(Creatures interactor)
    {
        if (!_locked)
        {
            if (_destination != null)
            {
                interactor.transform.position = _destination.transform.position;

            }
            else if (_teleportScene != null)
            {
                Debug.Log("Scene teleport requested");

                interactor.gameObject.SetActive(false);
                SystemController.LoadScene(_teleportScene);
            }
        }
    }

    public void OnTrigger()
    {
        if (_locked)
        {
            _locked = false;
            _rendererBot.sprite = _unlockedSpriteBottom;
            _rendererTop.sprite = _unlockedSpriteTop;
        }
        else
        {
            _locked = true;
            _rendererBot.sprite = _lockedSpriteBottom;
            _rendererTop.sprite = _lockedSpriteTop;
        }

    }
}
