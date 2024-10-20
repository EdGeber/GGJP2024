using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

#pragma warning disable UNT0008

[RequireComponent(typeof(StateTokenComponent))]
public sealed class LevelsManager : MonoBehaviour
{
    CancellationToken AliveToken => GetComponent<StateTokenComponent>().AliveToken;

    Level currLevel;
    List<Level> levels;


    void Awake()
    {
        Level[] arr = GetComponents<Level>();
        levels = Enumerable.Repeat<Level>(null, arr.Length).ToList();
        foreach (var level in arr)
        {
            levels[level.LevelNumber] = level;
        }
    }

    void Subscribe()
    {
        GameEvents.CurrentLevel.AddListener(OnCurrentLevelNumberChanged, AliveToken, true);
    }

    void OnCurrentLevelNumberChanged(int currLevelNumber)
    {
        if (currLevelNumber > GameEvents.MaxLevelReached.Value)
        {
            GameEvents.MaxLevelReached.Set(currLevelNumber);
        }
        if (currLevel != null)
        {
            currLevel.EndLevel();
        }
        if (currLevelNumber >= levels.Count)
        {
            return;
        }
        currLevel = levels[currLevelNumber];
        currLevel.StartLevel();
    }

    void Start()
    {
        Subscribe();
    }

}
