using UnityEngine;
using NaughtyAttributes;
public abstract class Level : MonoBehaviour
{

    [SerializeField]
    IntEvent tabletBattery;

    [ShowNativeProperty]
    public abstract int LevelNumber { get; }
    // [ShowNativeProperty]
    // public abstract string LevelName { get; }

    public virtual void StartLevel()
    {
        GameEvents.MaxTabletBattery.Set(tabletBattery.Value);
    }

    public abstract void EndLevel();

}