using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using NaughtyAttributes;

public abstract class GameEvent<T> : GameEventBase
{
    [SerializeField]
    [Tooltip("The value this variable assumes in a fresh game save.")]
    protected T defaultValue = default;

    [ReadOnly]
    [SerializeField]
    [Tooltip("The current value. It is passed to the listeners when you click Raise.")]
    protected T currentValue = default;

    [SerializeField]
    [Tooltip("Change this value and click \"Set New Value\" in order to invoke OldNewValueListeners. If the new value is equal to the current value, the listeners won't be called.")]
    protected T newValue = default;

    // must Raise() in case currentValue changes
    public virtual T Value
    {
        get => currentValue;
        set
        {
            if (!EqualityComparer<T>.Default.Equals(this.currentValue, value))
            {
                T oldValue = currentValue;
                currentValue = value;
                event_?.Invoke(currentValue);
                oldNewEvent?.Invoke(this, oldValue, currentValue);
            }
        }
    }

    // an event with the new value
    event System.Action<T> event_;

    // an event with the ValueReference, its new value and its old value
    public delegate void OldNewValueListener(
        GameEvent<T> variable, T oldValue, T newValue
    );
    event OldNewValueListener oldNewEvent;

    /// <summary>
    /// Reset the variable to its default value, as specified in the inspector.
    /// </summary>
    /// <param name="silent">Whether to suppres the value change event.</param>
    public void ResetToDefault(bool silent=true)
    {
        Set(defaultValue, silent);
    }

    public override void ResetToDefault()
    {
        ResetToDefault(true);
    }

    /// <summary>
    /// Set the value of the variable and optionally raise value change events.
    /// </summary>
    /// <param name="silent">If true, value change events are not raised.</param>
    public virtual void Set(T newValue, bool silent = false)
    {
        if (silent)
        {
            currentValue = newValue;
        }
        else
        {
            Value = newValue;
        }
    }

    public void SetBoxedValue(object boxedValue)
    {
        if (boxedValue is T value)
        {
            Set(value);
        }
        else
        {
            Debug.LogWarning(
                $"Error at {name}.SetBoxedValue: cannot assign {boxedValue.GetType().Name} " +
                $"to a ValueReference<{typeof(T).Name}>."
            );
        }
    }

    /// <summary>
    /// Assigns `newValue` to `Value`.
    /// </summary>
    [Button]
    public void SetNewValue()
    {
        Value = newValue;
    }

    /// <summary>
    /// Raise the value change event using the variable's current value.
    /// </summary>
    [Button]
    public virtual void Raise()
    {
        event_?.Invoke(currentValue);
    }

    /// <summary>
    /// Raise a value change event using the passed value, but don't
    /// change the variable's current value. This is useful if you only
    /// care about firing valued events, but don't need to persist the value.
    /// </summary>
    public virtual void Raise(T newValue)
    {
        event_?.Invoke(newValue);
    }

    /// <summary>
    /// Subscribe a listener that is called when the value changes.
    /// </summary>
    /// <param name="callImmediately">Whether to call the listener immediately after it is added.</param>
    public virtual System.Action<T> AddListener(
        System.Action<T> lis,
        bool callImmediately = false
    )
    {
        event_ -= lis;
        event_ += lis;
        if (callImmediately) lis(Value);
        return lis;
    }

    /// <summary>
    /// Subscribe a listener that is called when the value changes.
    /// </summary>
    /// <param name="cancellationToken">Cancelling this token removes the listener.</param>
    /// <param name="callImmediately">Whether to call the listener immediately after it is added.</param>
    public virtual void AddListener(
        System.Action<T> lis,
        CancellationToken cancellationToken,
        bool callImmediately = false
    )
    {
        AddListener(lis, callImmediately);
        cancellationToken.Register(delegate { RemoveListener(lis); });
    }

    /// <summary>
    /// Subscribe a listener that is called when the value changes. The listener
    /// unsubscribes itself after being called the first time.
    /// </summary>
    /// <returns>A CancellationTokenSource that can be used to cancel the subscription.</returns>    
    public virtual CancellationTokenSource AddOnceListener(
        System.Action<T> lis
    )
    {
        CancellationTokenSource cts = new();
        void wrapper(T val)
        {
            cts.Cancel();
            cts.Dispose();
            lis(val);
        }
        AddListener(wrapper, cts.Token, false);
        return cts;
    }

    /// <summary>
    /// Subscribe a listener that is called when the value changes. The listener
    /// unsubscribes itself after being called the first time.
    /// </summary>
    /// <param name="cancellationToken">Cancelling this token removes the listener.</param>
    public virtual void AddOnceListener(
        System.Action<T> lis,
        CancellationToken cancellationToken
    )
    {
        CancellationTokenSource cts = new();
        cancellationToken.Register(delegate
        {
            if (cts.IsCancellationRequested) return;
            cts.Cancel();
            cts.Dispose();
        });
        void wrapper(T val)
        {
            cts.Cancel();
            cts.Dispose();
            lis(val);
        }
        AddListener(wrapper, cts.Token, false);
    }

    public virtual void RemoveListener(System.Action<T> lis) => event_ -= lis;

    /// <summary>
    /// Subscribe a listener that is called when the value changes. The listener
    /// receives both the previous and the new values of the variable.
    /// </summary>
    /// <param name="lis"></param>
    /// <returns></returns>
    public virtual OldNewValueListener AddListener(OldNewValueListener lis)
    {
        oldNewEvent -= lis;
        oldNewEvent += lis;
        return lis;
    }

    public virtual void RemoveListener(OldNewValueListener lis)
    {
        oldNewEvent -= lis;
    }

    protected bool IsEventNull() => event_ == null;

    public virtual void RemoveAllListeners()
    {
        event_ = null;
        oldNewEvent = null;
    }

}