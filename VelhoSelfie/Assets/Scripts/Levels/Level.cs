using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;
public abstract class Level : MonoBehaviour
{

    [SerializeField]
    IntEvent tabletBattery;

    [ShowNativeProperty]
    public abstract int LevelNumber { get; }
    // [ShowNativeProperty]
    // public abstract string LevelName { get; }

    InputActions inputActions;

    public virtual void StartLevel()
    {
        SetupEvents();
    }

    public virtual void EndLevel() { }

    protected virtual void Awake()
    {
        inputActions = new InputActions();
        inputActions.Global.Enable();
        StartLevel();
    }
    
    protected virtual void SetupEvents()
    {
        GameEvents.MaxTabletBattery.Set(tabletBattery.Value);
    }

    protected virtual void Update()
    {
        if(inputActions.Global.RestartLevel.triggered)
        {
            enabled = false;
            CurtainManager.Instance.FadeIn(() =>
            {
                SceneManager.LoadScene($"Level{LevelNumber}");
                SetupEvents();
                CurtainManager.Instance.FadeOut();
            });
        }
    }

}