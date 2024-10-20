using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurtainManager : MonoBehaviour
{
    public static CurtainManager Instance { get; private set; }

    [SerializeField] Image panel;
    [SerializeField] TweenSettings<float> fadeTweenSettings;
    Tween tween;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public void FadeIn(System.Action onComplete = null)
    {
        tween.Stop();
        fadeTweenSettings.startValue = 0f;
        fadeTweenSettings.endValue = 1f;
        tween = Tween.Alpha(panel, fadeTweenSettings);
        if(onComplete != null )
        {
            tween.OnComplete(onComplete);
        }
    }

    public void FadeOut(System.Action onComplete = null)
    {
        tween.Stop();
        fadeTweenSettings.startValue = 1f;
        fadeTweenSettings.endValue = 0f;
        tween = Tween.Alpha(panel, fadeTweenSettings);
        if(onComplete != null )
        {
            tween.OnComplete(onComplete);
        }
    }
}
