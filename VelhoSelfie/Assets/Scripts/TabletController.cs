using System.Collections;
using System.Collections.Generic;
using System.Threading;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(StateTokenComponent))]
public class TabletController : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 1f)]
    float offColor = 0.5f;

    [SerializeField]
    TMP_Text batteryValue;

    [SerializeField]
    RectTransform cameraTexture;

    [SerializeField]
    RectTransform restTransform; // t = 0

    [SerializeField]
    RectTransform focusTransform; // t = 1

    [SerializeField]
    TweenSettings<float> tweenSettings;

    RawImage cameraImage;

    CancellationToken AliveToken => GetComponent<StateTokenComponent>().AliveToken;
    Tween tween;
    float currTweenVal = 0f;

    void OnTween(float t)
    {
        currTweenVal = t;
        cameraTexture.SetLocalPositionAndRotation(
            Vector3.Lerp(restTransform.localPosition, focusTransform.localPosition, t),
            Quaternion.Lerp(restTransform.localRotation, focusTransform.localRotation, t)
        );
        float color = (1 - t) * offColor + t;
        cameraImage.color = new Color(color, color, color);
    }

    void OnPlayerLookedDown(bool lookedDown)
    {
        // if (GameEvents.CurrentTabletBattery.Value == 0)
        // {
        //     return;
        // }
        if (lookedDown)
        {
            tweenSettings.startValue = currTweenVal;
            tweenSettings.endValue = 1f;
        }
        else
        {
            tweenSettings.startValue = currTweenVal;
            tweenSettings.endValue = 0f;
            GameEvents.CurrentTabletBattery.Value = Mathf.Max(
                GameEvents.CurrentTabletBattery.Value - 1,
                0
            );
            if (GameEvents.CurrentTabletBattery.Value == 0)
            {
                offColor = 0.25f;
            }
        }
        tween.Stop();
        tween = Tween.Custom(tweenSettings, OnTween);
    }

    void UpdateBatteryValue(int ignore)
    {
        batteryValue.text =
            $"{GameEvents.CurrentTabletBattery.Value}/{GameEvents.MaxTabletBattery.Value}";
    }

    void Awake()
    {
        cameraImage = cameraTexture.gameObject.GetComponent<RawImage>();
    }

    void Start()
    {
        GameEvents.CurrentTabletBattery.Set(GameEvents.MaxTabletBattery.Value);
        GameEvents.PlayerIsLookingDown.AddListener(OnPlayerLookedDown, AliveToken);
        GameEvents.MaxTabletBattery.AddListener(UpdateBatteryValue, AliveToken);
        GameEvents.CurrentTabletBattery.AddListener(UpdateBatteryValue, AliveToken, true);
        OnTween(0f);
    }
}
