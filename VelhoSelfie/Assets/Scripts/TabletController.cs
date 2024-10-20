using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(StateTokenComponent))]
public class TabletController : MonoBehaviour
{
    [SerializeField] TMP_Text batteryValue;
    [SerializeField] RectTransform cameraTexture;
    [SerializeField] RectTransform restTransform;  // t = 0
    [SerializeField] RectTransform focusTransform; // t = 1
    [SerializeField] TweenSettings<float> tweenSettings;
 
    CancellationToken AliveToken => GetComponent<StateTokenComponent>().AliveToken;
    Tween tween;
    float currTweenVal = 0f;

    void OnTween(float t)
    {
        currTweenVal = t;
        cameraTexture.SetLocalPositionAndRotation(
            Vector3.Lerp(restTransform.localPosition, focusTransform.localPosition, t),
            Quaternion.Slerp(restTransform.localRotation, focusTransform.localRotation, t)
        );
    }

    void OnPlayerLookedDown(bool lookedDown)
    {
        if (lookedDown)
        {
            tweenSettings.startValue = currTweenVal;
            tweenSettings.endValue = 1f;
        } else
        {
            tweenSettings.startValue = currTweenVal;
            tweenSettings.endValue = 0f;
        }
        tween.Stop();
        tween = Tween.Custom(tweenSettings, OnTween);
    }

    void UpdateBatteryValue(int ignore)
    {
        batteryValue.text = $"{GameEvents.CurrentTabletBattery}/{GameEvents.MaxTabletBattery}";
    }

    void Start()
    {
        GameEvents.PlayerIsLookingDown.AddListener(OnPlayerLookedDown, AliveToken);
        GameEvents.MaxTabletBattery.AddListener(UpdateBatteryValue, AliveToken);
        GameEvents.CurrentTabletBattery.AddListener(UpdateBatteryValue, AliveToken );
    }

}
