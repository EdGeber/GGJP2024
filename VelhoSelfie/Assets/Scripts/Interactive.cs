using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Interactive : MonoBehaviour
{
    public UnityEvent onFocused;
    public UnityEvent onUnfocused;
    public UnityEvent onInteracted;

    public bool IsInteractive { get; set; }
    public bool PlayerEverInteractedWith { get; set; }

    void SetupInteractionColliders()
    {
        int numColliders = Utils.DoOnDescendants(
            transform,
            t => t.GetComponent<InteractionCollider>().parentInteractive = this,
            t => t.TryGetComponent<InteractionCollider>(out var _)
        );
        if (numColliders == 0)
        {
            Debug.LogWarning(
                $"No interaction colliders have been found under interactive " +
                $"object {name}. The player won't be able to interact with this " +
                $"object. Please fix this by adding a child GameObject of {name} " +
                $"with a InteractionCollider component."
            );
        }
    }

    protected virtual void Awake()
    {
        SetupInteractionColliders();
    }

    public virtual void Focus()
    {
        onFocused?.Invoke();
    }

    public virtual void Unfocus()
    {
        onUnfocused?.Invoke();
    }

    public void TryInteract()
    {
        if (IsInteractive) OnInteract();
        else OnFailedInteractionAttempt();
    }

    protected virtual void OnInteract()
    {
        onInteracted?.Invoke();
    }

    protected virtual void OnFailedInteractionAttempt()
    {

    }


}
