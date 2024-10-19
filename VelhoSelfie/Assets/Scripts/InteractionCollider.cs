using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionCollider : MonoBehaviour
{
    [System.NonSerialized]  // set by Interactive.cs
    public Interactive parentInteractive;

    private void Reset()
    {
        gameObject.layer = LayerMask.NameToLayer("InteractionCollider");
    }

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("InteractionCollider");
        GetComponent<Collider>().isTrigger = false;
    }
}
