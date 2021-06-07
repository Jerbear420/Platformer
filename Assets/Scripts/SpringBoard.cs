using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Interactable))]
public class SpringBoard : RaycastController, IInteractable
{
    [SerializeField] private bool _passThrough;
    [SerializeField] private Sprite _downImg;
    [SerializeField] private Sprite _upImg;
    [SerializeField] private float _power;
    public bool PassThrough { get { return _passThrough; } }

    private Interactable _interactable;
    public Interactable Interactable { get { return _interactable; } }
    private bool _triggered;
    [SerializeField] private float delayTime;
    private SpriteRenderer _renderer;
    private CollisionInfo _collisionInfo;
    void Awake()
    {
        _triggered = false;
        _collisionInfo = new CollisionInfo();
        _interactable = GetComponent<Interactable>();
        _interactable.RegisterInteraction(Interact, Nearby);
        _collisionInfo.faceDir = 1;
        _collisionInfo.Reset();
        _renderer = GetComponent<SpriteRenderer>();
    }
    public void Nearby(Creatures interactor)
    {

        if (!_triggered)
        {
            Debug.Log("Trigger time!");
            UpdateRaycastOrigins();
            VerticleCollisions();
        }
    }
    public void Interact(Creatures interactor)
    {

    }
    private void VerticleCollisions()
    {
        float rayLength = 1 + skinWidth;
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticleRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, collisionMask);
            if (hit)
            {
                if (!_triggered)
                {
                    if (hit.transform.tag == "Player")
                    {
                        Trigger();
                    }
                }
                else
                {
                    if (hit.transform.tag == "Player")
                    {
                        Creatures player = hit.transform.gameObject.GetComponent<Creatures>();
                        player.Jump(_power);
                    }
                }
            }

        }
    }
    private void Trigger()
    {
        if (!_triggered)
        {
            _triggered = true;

            Invoke("Bounce", delayTime);
            _renderer.sprite = _downImg;
            _collider.offset = new Vector2(0, -0.33f);
            UpdateRaycastOrigins();
        }
    }
    private void Bounce()
    {
        VerticleCollisions();
        _renderer.sprite = _upImg;
        _collider.offset = new Vector2(0, -0.1535392f);
        UpdateRaycastOrigins();
        _triggered = false;
    }
}