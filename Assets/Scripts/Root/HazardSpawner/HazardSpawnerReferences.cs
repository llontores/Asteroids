using UnityEngine;

public class HazardSpawnerReferences : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;
    
    [Header("Prefabs")]
    [SerializeField] private UFO _ufoPrefab;
    [SerializeField] private Asteroid _asteroidPrefab;
    [SerializeField] private Fragment _fragmentPrefab;

    [Header("Containers")]
    [SerializeField] private Transform _ufoContainer;
    [SerializeField] private Transform _asteroidContainer;
    [SerializeField] private Transform _fragmentContainer;

    public UFO UfoPrefab => _ufoPrefab;
    public Asteroid AsteroidPrefab => _asteroidPrefab;
    public Fragment FragmentPrefab => _fragmentPrefab;
    public Transform UfoContainer => _ufoContainer;
    public Transform AsteroidContainer => _asteroidContainer;
    public Transform FragmentContainer => _fragmentContainer;
    
    public Transform GetRandomSpawnPoint() 
        => _spawnPoints[Random.Range(0, _spawnPoints.Length)];

    public void Place(Transform obj, Transform point)
    {
        obj.position = point.position;
        obj.rotation = point.rotation;
        obj.gameObject.SetActive(true);
    }
}