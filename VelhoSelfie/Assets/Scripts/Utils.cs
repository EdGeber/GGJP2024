using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
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
