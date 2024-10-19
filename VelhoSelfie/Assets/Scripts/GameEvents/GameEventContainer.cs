using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "GameEventContainer", menuName = "Game Event/Container")]
public sealed class GameEventContainer : ScriptableObject
{
    public static GameEventContainer Instance;
    public static T Get<T>(string eventName) where T : GameEventBase
    {
        if (Instance == null)
        {
            Debug.LogWarning(
                $"Couldn't retrieve variable {eventName} because " +
                "the GameVars singleton is not set."
            );
            return null;
        }
        Instance.dict.TryGetValue(eventName, out GameEventBase gameEventBase);
        if (gameEventBase is not T gameEvent)
        {
            Debug.LogWarning(
                $"Couldn't retrieve variable {eventName} of type " +
                $"{typeof(T).FullName} because it is not in the variable container. " +
                $"Maybe you forgot to add it to the variable container, or the type is " +
                $"wrong."
            );
            return null;
        }
        return gameEvent;
    }

    [ShowNativeProperty]
    bool SingletonSet => Instance != null;

    [SerializeField]
    List<GameEventBase> list;
    Dictionary<string, GameEventBase> dict = new();

    void OnValidate()
    {
        if (list == null) return;
        for (int i = 0; i < list.Count; i++)
        {
            var v = list[i];
            if (i > 0 && System.Object.ReferenceEquals(v, list[i - 1]))
            {
                list[i] = null;
            }
        }
    }

    [Button]
    public void ResetAllToDefault()
    {
        list?.ForEach(e =>
        {
            e.ResetToDefault();
        });
    }

    public void OnEnable()
    {
        Instance = this;
        dict = new();
        list?.ForEach(e =>
        {
            dict[e.name] = e;
        });
    }
}