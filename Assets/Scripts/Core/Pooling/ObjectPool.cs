using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ObjectPool<T> where T : Component
{
    private int _capacity;
    private Transform _container;
    
    private IFactory<T> _factory;
    private T _prefab;
    private bool _useFactory;

    public Transform Container => _container;
    private readonly Queue<T> _pool = new Queue<T>();
    
    public ObjectPool(int capacity, IFactory<T> factory, Transform container)
    {
        _capacity = capacity;
        _factory = factory;
        _container = container;
        _useFactory = true;
        
        InitializePool();
    }
    
    public ObjectPool(int capacity, T prefab, Transform container)
    {
        _capacity = capacity;
        _prefab = prefab;
        _container = container;
        _useFactory = false;
        
        InitializePool();
    }
    
    private void InitializePool()
    {
        for (int i = 0; i < _capacity; i++)
        {
            T spawned = _useFactory ? _factory.Create() : UnityEngine.Object.Instantiate(_prefab);
            
            spawned.transform.SetParent(_container);
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