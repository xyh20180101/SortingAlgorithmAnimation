using System.Collections;

namespace SortingAlgorithmAnimation.SortingAlgorithms;

/// <summary>
/// 自定义的列表结构
/// <para>提供了索引器读写事件,Swap方法,辅助集合申请</para>
/// </summary>
/// <typeparam name="T"></typeparam>
public class ListeningList<T> : IList<T>
{
    private readonly List<T> _list;

    public ListeningList()
    {
        _list = new List<T>();
    }

    public ListeningList(IList<T> list)
    {
        _list = list.ToList();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    public void Add(T item)
    {
        _list.Add(item);
    }

    public void Clear()
    {
        _list.Clear();
    }

    public bool Contains(T item)
    {
        return _list.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        return _list.Remove(item);
    }

    public int Count => _list.Count;
    public bool IsReadOnly => false;

    public int IndexOf(T item)
    {
        return _list.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        _list.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        _list.RemoveAt(index);
    }

    public T this[int index]
    {
        get
        {
            var v = _list[index];
            ReadEvent?.Invoke(index, v);
            return v;
        }
        set
        {
            var old = _list[index];
            _list[index] = value;
            WriteEvent?.Invoke(index, old, value);
        }
    }

    public event Action<int, T>? ReadEvent;

    public event Action<int, T, T>? WriteEvent;

    public event Action<int, int, T, T, T, T>? SwapEvent;

    public event Action<int>? ExternalSpaceEvent;

    public void Swap(int index1, int index2)
    {
        var oldValue1 = _list[index1];
        var oldValue2 = _list[index2];
        (_list[index1], _list[index2]) = (_list[index2], _list[index1]);
        SwapEvent?.Invoke(index1, index2, oldValue1, oldValue2, _list[index1], _list[index2]);
    }

    public T[] ExternalArray(int length)
    {
        ExternalSpaceEvent?.Invoke(length);
        return new T[length];
    }

    public ExternalList<T> ExternalList()
    {
        var list = new ExternalList<T>();
        list.AddEvent += () => ExternalSpaceEvent?.Invoke(1);
        return list;
    }
}