using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;
public abstract class Level : MonoBehaviour
{

    [SerializeField]
    IntEvent tabletBattery;
    [SerializeField]
    bool isFinalLevel;

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

    public virtual void OnPlayerEnteredTrigger()
    {
        enabled = false;
        CurtainManager.Instance.FadeIn(() =>
        {
            if (isFinalLevel)
            {
                SceneManager.LoadScene("EndGame");
            } else
            {
                SceneManager.LoadScene($"Level{LevelNumber + 1}");
            }
            CurtainManager.Instance.FadeOut();
        });
    }
}