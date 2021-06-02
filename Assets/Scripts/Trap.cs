using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Trap : MonoBehaviour
{
    public static Dictionary<Transform, Trap> AllTraps = new Dictionary<Transform, Trap>();
    [SerializeField] private float _damage;

    public float Damage { get { return _damage; } }
    void Awake()
    {
        AllTraps.Add(transform, this);
    }
}
