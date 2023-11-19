using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SerializableDictionary<TK, TV> : ISerializationCallbackReceiver,IDictionary<TK, TV>
{
    private Dictionary<TK, TV> _Dictionary;
    [SerializeField] List<TK> _Keys;
    [SerializeField] List<TV> _Values;

    public void OnBeforeSerialize()
    {
        _Keys = new List<TK>(_Dictionary.Keys);
        _Values = new List<TV>(_Dictionary.Values);
    }

    public void OnAfterDeserialize()
    {
        var count = Mathf.Min(_Keys.Count, _Values.Count);
        _Dictionary = new Dictionary<TK, TV>(count);
        for (var i = 0; i < count; ++i)
        {
            _Dictionary.Add(_Keys[i], _Values[i]);
        }
    }

    public IEnumerator<KeyValuePair<TK, TV>> GetEnumerator()
    {
        return _Dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(KeyValuePair<TK, TV> item)
    {
        this._Dictionary.Add(item.Key, item.Value);
    }

    public void Clear()
    {
        this._Dictionary.Clear();
    }

    public bool Contains(KeyValuePair<TK, TV> item)
    {
        return this._Dictionary.Contains(item);
    }

    public void CopyTo(KeyValuePair<TK, TV>[] array, int arrayIndex)
    {
        throw new System.NotImplementedException();
    }

    public bool Remove(KeyValuePair<TK, TV> item)
    {
        return this._Dictionary.Remove(item.Key);
    }

    public int Count { get; }
    public bool IsReadOnly { get; }
    public void Add(TK key, TV value)
    {
        this._Dictionary.Add(key, value);
    }

    public bool ContainsKey(TK key)
    {
        return this._Dictionary.ContainsKey(key);
    }

    public bool Remove(TK key)
    {
        return this._Dictionary.Remove(key);
    }

    public bool TryGetValue(TK key, out TV value)
    {
        return this._Dictionary.TryGetValue(key, out value);
    }

    public TV this[TK key]
    {
        get => this._Dictionary[key];
        set => this._Dictionary[key] = value;
    }

    public ICollection<TK> Keys { get; }
    public ICollection<TV> Values { get; }
}