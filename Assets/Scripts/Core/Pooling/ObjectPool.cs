using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private int _capacity;
    private Transform _container;
    
    public Transform Container => _container;

    private readonly Queue<T> _pool = new Queue<T>();

    public ObjectPool(int capacity, T prefab, Transform container)
    {
        _capacity = capacity;
        
        _container = container;

        for (int i = 0; i < _capacity; i++)
        {
            T spawned = UnityEngine.Object.Instantiate(prefab, _container);
            spawned.gameObject.SetActive(false);

            _pool.Enqueue(spawned);
        }
    }

    public bool TryGetObject(out T result)
    {
        result = _pool.Count > 0 ? _pool.Dequeue() : null;
        return result != null;
    }

    public void ResetPool()
    {
        _pool.Clear();

        foreach (Transform child in _container)
        {
            T component = child.GetComponent<T>();

            if (component != null)
            {
                component.gameObject.SetActive(false);
                _pool.Enqueue(component);
            }
        }
    }

    public void ReturnObject(T returnedObject)
    {
        returnedObject.gameObject.SetActive(false);
        _pool.Enqueue(returnedObject);
    }
}