using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(StateTokenComponent))]
public class TabletController : MonoBehaviour
{
    [SerializeField] RectTransform cameraTexture;
    [SerializeField] RectTransform restTransform;
    [SerializeField] RectTransform focusTransform;
    [SerializeField] TweenSettings<float> tweenSettings;
 
    CancellationToken AliveToken => GetComponent<StateTokenComponent>().AliveToken;
    Tween tween;

    void OnTween(float t)
    {
        cameraTexture.transform.SetLocalPositionAndRotation(
            Vector3.Lerp(restTransform.localPosition, focusTransform.localPosition, t),
            Quaternion.Slerp(restTransform.localRotation, focusTransform.localRotation, t)
        );
    }

    void OnPlayerLookedDown(bool lookedDown)
    {
        if (lookedDown)
        {
            tweenSettings.startValue = tween.isAlive ? tween.progress : 0f;
        } else
        {
            tweenSettings.startValue = tween.isAlive ? tween.progress : 1f;
        }
        tween.Stop();
        tween = Tween.Custom(tweenSettings, OnTween);
    }

    void Start()
    {
        GameEvents.PlayerIsLookingDown.AddListener(OnPlayerLookedDown, AliveToken);
    }

}
