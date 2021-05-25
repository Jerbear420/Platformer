using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Containers : MonoBehaviour
{
    [Range(1, 100)]
    [SerializeField] private int coins;

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("AAA");
    }
}
