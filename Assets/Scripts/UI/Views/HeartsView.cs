using System.Collections.Generic;
using UnityEngine;

public class HeartsView : MonoBehaviour
{
    [SerializeField] private GameObject _heartPrefab; 
    [SerializeField] private Transform _container;    

    private readonly List<GameObject> _hearts = new List<GameObject>();

    public void UpdateView(HealthData data)
    {
        if (_hearts.Count != data.Max)
        {
            foreach (var heart in _hearts) Destroy(heart);
            _hearts.Clear();

            for (int i = 0; i < data.Max; i++)
            {
                _hearts.Add(Instantiate(_heartPrefab, _container));
            }
        }

        for (int i = 0; i < _hearts.Count; i++)
        {
            _hearts[i].SetActive(i < data.Current);
        }
    }
}