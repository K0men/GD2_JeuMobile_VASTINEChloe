using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _collectiblePrefab;
    [SerializeField] private Transform[] _lanePositions;
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private float _spawnY = 8f;

    private float _timer = 0f;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < _spawnInterval) return;
        _timer = 0f;
        SpawnCollectible();
    }

    private void SpawnCollectible()
    {
        int lane = Random.Range(0, _lanePositions.Length);
        Vector3 spawnPos = new Vector3(_lanePositions[lane].position.x, _spawnY, 0f);
        Instantiate(_collectiblePrefab, spawnPos, Quaternion.identity);
    }
}
