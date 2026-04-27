using UnityEngine;

public class HazardSpawnerView : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;
    
    [Header("Prefabs")]
    public UFO UfoPrefab;
    public Asteroid AsteroidPrefab;
    public Fragment FragmentPrefab;

    [Header("Containers")]
    public Transform UfoContainer;
    public Transform AsteroidContainer;
    public Transform FragmentContainer;

    public Transform GetRandomSpawnPoint() 
        => _spawnPoints[Random.Range(0, _spawnPoints.Length)];

    public void Place(Transform obj, Transform point)
    {
        obj.position = point.position;
        obj.rotation = point.rotation;
        obj.gameObject.SetActive(true);
    }
}