using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Events;
using UnityEngine.UIElements;

[RequireComponent(typeof(Outline))]
public class Interactive : MonoBehaviour
{
    public UnityEvent onFocused;
    public UnityEvent onUnfocused;
    public UnityEvent onInteracted;

    public bool IsFocused { get; protected set; } = false;
    public bool IsInteractive { get; set; } = true;
    public bool PlayerEverInteractedWith { get; set; } = false;

    Outline Outline => GetComponent<Outline>();

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
                $"No interaction colliders have been found under interactive "
                    + $"object {name}. The player won't be able to interact with this "
                    + $"object. Please fix this by adding a child GameObject of {name} "
                    + $"with a InteractionCollider component."
            );
        }
    }

    protected virtual void Awake()
    {
        SetupInteractionColliders();
    }

    private void FixedUpdate()
    {
        if (GameManagerController.instance.IsIn2D)
            Outline.enabled = false;
    }

    public virtual void Focus()
    {
        Debug.Assert(IsInteractive, "Voc� n�o deve tentar focar um objeto que n�o � interativo.");
        IsFocused = true;
        onFocused?.Invoke();
        Outline.enabled = true;
        GameEvents.ObjectFocusChanged?.Raise(this);
    }

    public virtual void Unfocus()
    {
        IsFocused = false;
        onUnfocused?.Invoke();
        Outline.enabled = false;
        GameEvents.ObjectFocusChanged?.Raise(this);
    }

    public virtual void Interact()
    {
        if (!IsInteractive)
        {
            Debug.LogWarning("N�o tente interagir com objetos que t�m obj.IsInteractive == false");
            return;
        }
        PlayerEverInteractedWith = true;
        onInteracted?.Invoke();
    }
}
