using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] _Fish;
    [SerializeField] private GameObject[] _Octopus;
    [SerializeField] private GameObject[] _Crab;

    [Header("Spawn Quotas")]
    [SerializeField] private int _maxFish = 5;
    [SerializeField] private int _maxOctopus = 3;
    [SerializeField] private int _maxCrab = 2;

    [Header("Spawn Settings")]
    [SerializeField] private Transform[] _lanePositions;
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private float _spawnY = 8f;

    private int _spawnedFish;
    private int _spawnedOctopus;
    private int _spawnedCrab;
    private float _timer = 0f;
    private bool _done = false;

    public void SetSpawnY(float y) => _spawnY = y;

    private void Update()
    {
        if (_done) return;

        _timer += Time.deltaTime;
        if (_timer < _spawnInterval) return;
        _timer = 0f;
        SpawnCollectible();
    }

    private void SpawnCollectible()
    {
        List<GameObject> pool = new List<GameObject>();

        if (_spawnedFish < _maxFish && _Fish.Length > 0)
            pool.Add(_Fish[Random.Range(0, _Fish.Length)]);
        if (_spawnedOctopus < _maxOctopus && _Octopus.Length > 0)
            pool.Add(_Octopus[Random.Range(0, _Octopus.Length)]);
        if (_spawnedCrab < _maxCrab && _Crab.Length > 0)
            pool.Add(_Crab[Random.Range(0, _Crab.Length)]);

        if (pool.Count == 0) { _done = true; return; }

        GameObject chosen = pool[Random.Range(0, pool.Count)];

        if (chosen == _Fish[0]) _spawnedFish++;
        else if (chosen == _Octopus[0]) _spawnedOctopus++;
        else _spawnedCrab++;

        int lane = Random.Range(0, _lanePositions.Length);
        Vector3 spawnPos = new Vector3(_lanePositions[lane].position.x, _spawnY, 0f);
        Instantiate(chosen, spawnPos, Quaternion.identity);
    }
}
