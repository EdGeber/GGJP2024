using UnityEngine;
using System.Threading;

public sealed class StateTokenComponent : MonoBehaviour
{
    CancellationTokenSource aliveTokenSource;
    public CancellationToken AliveToken
    {
        get
        {
            aliveTokenSource ??= new CancellationTokenSource();  // if null assign
            return aliveTokenSource.Token;
        }
    }

    CancellationTokenSource enabledTokenSource;
    public CancellationToken EnabledToken
    {
        get
        {
            enabledTokenSource ??= new CancellationTokenSource();  // if null assign
            return enabledTokenSource.Token;
        }
    }

    private void OnDisable()
    {
        if (enabledTokenSource == null) return;
        enabledTokenSource.Cancel();
        enabledTokenSource.Dispose();
        enabledTokenSource = null;
    }

    void OnDestroy()
    {
        if (aliveTokenSource == null) return;
        aliveTokenSource.Cancel();
        aliveTokenSource.Dispose();
        aliveTokenSource = null;
    }
}