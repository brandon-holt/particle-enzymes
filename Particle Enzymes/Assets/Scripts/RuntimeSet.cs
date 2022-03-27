using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeSet<T> : ScriptableObject
{
    public List<T> items = new List<T>();

    public void Add(T item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);
        }
    }

    public void Remove(T item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
        }
    }

    public void Clear()
    {
        items.Clear();
    }
}
