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

    public bool IsFocused { get; protected set; } = false;
    public bool IsInteractive { get; set; } = true;
    public bool PlayerEverInteractedWith { get; set; } = false;

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
        Debug.Assert(IsInteractive, "Você não deve tentar focar um objeto que não é interativo.");
        IsFocused = true;
        onFocused?.Invoke();
        Debug.Log("focus");
    }

    public virtual void Unfocus()
    {
        IsFocused = false;
        onUnfocused?.Invoke();
        Debug.Log("unfocus");
    }

    public virtual void Interact()
    {
        if(!IsInteractive)
        {
            Debug.LogWarning("Não tente interagir com objetos que têm obj.IsInteractive == false");
            return;
        }
        PlayerEverInteractedWith = true;
        onInteracted?.Invoke();
        Debug.Log("interact");
    }

}
