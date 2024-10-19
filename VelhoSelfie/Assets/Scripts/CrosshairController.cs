using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NaughtyAttributes;
using PrimeTween;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(StateTokenComponent))]
[RequireComponent(typeof(CanvasGroup))]
public sealed class CrosshairManager : MonoBehaviour
{
    CancellationToken AliveToken => GetComponent<StateTokenComponent>().AliveToken;
    CanvasGroup canvasGroup;
    Vector3 unfocusedCrosshairScale;
    Tween focusTween;
    float currFocusTweenVal = 0f;
    bool isFocused;
    Tween interactionTween;

    [Foldout("Focus animation")]
    [Range(1f, 3f)]
    [SerializeField]
    [Tooltip("The size of the crosshair in the focus state will be its initial size (when the game starts) times this value.")]
    float focusedSizeMultiplier = 1.9f;

    [Foldout("Focus animation")]
    [Range(0.1f, 1f)]
    [SerializeField]
    float focusAnimationDuration = 0.15f;

    [Foldout("Focus animation")]
    [Range(0.1f, 1f)]
    [SerializeField]
    float unfocusAnimationDuration = 0.4f;

    [Foldout("Focus animation")]
    [Range(0.1f, 1f)]
    [SerializeField]
    [Tooltip("The opacity of the crosshair layer when unfocused.")]
    float unfocusedOpacity = 0.3f;


    [Foldout("Interact animation")]
    [Range(0.25f, 0.95f)]
    [SerializeField]
    [Tooltip("The size of the crosshair upon interaction will become the current size times this value, and then it will get back to the original size.")]
    float interactionSizeMultiplier = 0.7f;

    [Foldout("Interact animation")]
    [Range(0.05f, 0.5f)]
    [SerializeField]
    float interactionAnimationDuration = 0.1f;

    [Foldout("Interact animation")]
    [SerializeField]
    Ease interactionEase = Ease.OutQuad;


    [Button]
    /// <summary>
    /// Fetch the values of Unfocused Crosshair Scale and Focused Crosshair Alphas
    /// </summary>
    void SetCurrentAnimationValuesAsInitial()
    {
        Canvas.ForceUpdateCanvases();
        unfocusedCrosshairScale = transform.localScale;
    }

    void OnFocusTween(float t)
    {
        // t == 1 means completely focused, t == 0 means completely unfocused
        currFocusTweenVal = t;
        transform.localScale = (1f - t) * unfocusedCrosshairScale + t * focusedSizeMultiplier * unfocusedCrosshairScale;
        canvasGroup.alpha = (1f - t) * unfocusedOpacity + t * 1f;
    }

    async void AnimateFocus(bool isFocused)
    {
        this.isFocused = isFocused;
        while (interactionTween.isAlive)
        {
            await interactionTween;
        }
        if (focusTween.isAlive)
        {
            focusTween.Stop();
        }
        if (isFocused)
        {
            focusTween = Tween.Custom(currFocusTweenVal, 1f, focusAnimationDuration, OnFocusTween, Ease.OutBack);
        }
        else
        {
            focusTween = Tween.Custom(currFocusTweenVal, 0f, unfocusAnimationDuration, OnFocusTween, Ease.OutExpo);
        }
    }

    void AnimateInteractionAttempt(Interactive interactive)
    {
        if (focusTween.isAlive)
        {
            focusTween.Stop();
        }
        if (interactionTween.isAlive)
        {
            interactionTween.Stop();
        }
        OnFocusTween(isFocused ? 1f : 0f);
        interactionTween = Tween.Scale(
            transform,
            endValue: interactionSizeMultiplier * transform.localScale,
            duration: interactionAnimationDuration,
            ease: interactionEase,
            cycles: 2,
            cycleMode: CycleMode.Yoyo
        );
    }

    void OnObjectFocusChanged(Interactive interactive)
    {
        AnimateFocus(interactive != null && interactive.IsFocused);
    }

    void OnViewChanged(View view)
    {
        if (view == View.FirstPerson)
        {
            gameObject.SetActive(true);
            GameEvents.ObjectFocusChanged?.AddListener(OnObjectFocusChanged, AliveToken, true);
            GameEvents.InteractionAttempted?.AddListener(AnimateInteractionAttempt, AliveToken);
        }
        else
        {
            gameObject.SetActive(false);
            GameEvents.ObjectFocusChanged?.RemoveListener(OnObjectFocusChanged);
            GameEvents.InteractionAttempted?.RemoveListener(AnimateInteractionAttempt);
        }
    }

    void Subscribe()
    {
        GameEvents.CurrentView?.AddListener(OnViewChanged, AliveToken, true);
    }

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        SetCurrentAnimationValuesAsInitial();
        Subscribe();
    }
}