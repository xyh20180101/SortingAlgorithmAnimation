namespace SortingAlgorithmAnimation.SortingAlgorithms;

public class ExternalList<T> : List<T>
{
    public event Action? AddEvent;

    public new void Add(T item)
    {
        base.Add(item);
        AddEvent?.Invoke();
    }
}