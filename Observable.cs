using System;
using System.Collections.Generic;

/// <summary>
/// This class stores a value of a specific type 
/// and sends a callback when a change occurs.
/// </summary>
/// <typeparam name="T">A type of value.</typeparam>
public class Observable<T>
{
    private T _value;
    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            NotifyPropertyChanged();
        }
    }

    private readonly List<Action<T>> _callbackList = new();

    public Observable() : this(default) { }

    public Observable(T value)
    {
        Value = value;
    }

    /// <summary>
    /// Start observing to detect changes in value.
    /// </summary>
    /// <param name="callback">A callback function.</param>
    public void Observe(Action<T> callback)
    {
        _callbackList.Add(callback);
    }

    /// <summary>
    /// Call callback to notify of changes in value.
    /// </summary>
    protected void NotifyPropertyChanged()
    {
        foreach (var callback in _callbackList)
        {
            callback.Invoke(Value);
        }
    }
}
