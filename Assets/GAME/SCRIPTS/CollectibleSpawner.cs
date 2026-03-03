using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _Fish;
    [SerializeField] private GameObject[] _Octopus;
    [SerializeField] private GameObject[] _Crab;
    public void SetSpawnY(float y) => _spawnY = y;
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
        GameObject[] allEnemies = new GameObject[_Fish.Length + _Octopus.Length + _Crab.Length];
        _Fish.CopyTo(allEnemies, 0);
        _Octopus.CopyTo(allEnemies, _Fish.Length);
        _Crab.CopyTo(allEnemies, _Fish.Length + _Octopus.Length);

        int lane = Random.Range(0, _lanePositions.Length);
        int enemyIndex = Random.Range(0, allEnemies.Length);
        Vector3 spawnPos = new Vector3(_lanePositions[lane].position.x, _spawnY, 0f);
        Instantiate(allEnemies[enemyIndex], spawnPos, Quaternion.identity);
    }
}
