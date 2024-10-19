using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Empty : MonoBehaviour { }

public static class Utils
{
    static MonoBehaviour instance;
    public static MonoBehaviour Instance
    {
        get
        {
            UnityEngine.Debug.Assert(
                Application.isPlaying,
                "Cannot access MonoBehaviour singleton out of play mode."
            );
            if (instance == null || !Application.IsPlaying(instance))
            {
                instance = new GameObject("Utils_Singleton").AddComponent<Empty>();
                UnityEngine.Object.DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }

    public static Coroutine StartCoroutine(IEnumerator routine)
    {
        return Instance.StartCoroutine(routine);
    }

    static IEnumerator _doAfter(int millis, System.Action action, bool scaledTime = false)
    {
        if (scaledTime)
            yield return new WaitForSeconds(((float)millis) / 1000f);
        else
            yield return new WaitForSecondsRealtime(((float)millis) / 1000f);
        action();
    }

    static IEnumerator _doAfter(
        int millis,
        System.Action action,
        CancellationToken cancellationToken,
        bool scaledTime = false
    )
    {
        if (scaledTime)
            yield return new WaitForSeconds(((float)millis) / 1000f);
        else
            yield return new WaitForSecondsRealtime(((float)millis) / 1000f);
        if (!cancellationToken.IsCancellationRequested)
        {
            action();
        }
    }

    public static void DoAfter(int millis, System.Action action, bool scaledTime = false)
    {
        StartCoroutine(_doAfter(millis, action, scaledTime));
    }

    public static void DoAfter(
        int millis,
        System.Action action,
        CancellationToken cancellationToken,
        bool scaledTime = false
    )
    {
        StartCoroutine(_doAfter(millis, action, cancellationToken, scaledTime));
    }

    /// <summary>
    /// Perform an action on all descendants of root (including the root itself)
    /// which satisfy the filter condition. If the filter is not provided, all
    /// descendants are included. The descendant tree traversal order is BFS.
    /// </summary>
    /// <returns>The number of objects to which the action was performed.</returns>
    public static int DoOnDescendants(
        Transform root,
        System.Action<Transform> action,
        System.Func<Transform, bool> filter = null
    )
    {
        filter ??= (_ => true);
        int actionCount = 0;
        Stack<Transform> stack = new();
        HashSet<Transform> visited = new();
        stack.Push(root);
        while (stack.Count > 0)
        {
            var curr = stack.Pop();
            if (visited.Contains(curr)) continue;
            visited.Add(curr);
            if (filter(curr))
            {
                action(curr);
                actionCount++;
            }
            foreach (Transform child in curr)
                stack.Push(child);
        }
        return actionCount;
    }
}
