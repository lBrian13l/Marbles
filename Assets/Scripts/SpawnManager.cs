using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _gemPrefab;

    private float _spawnRange = 45f;

    public GameObject SpawnAndGetPlayer(Vector3 spawnPosition)
    {
        GameObject playerObject = Instantiate(_playerPrefab, spawnPosition, _playerPrefab.transform.rotation);
        return playerObject;
    }

    public List<GameObject> SpawnAndGetGems(int enemyCount)
    {
        Vector3 spawnLocation;
        List<GameObject> gemObjects = new List<GameObject>();

        for (int i = 0; i < enemyCount; i++)
        {
            spawnLocation = new Vector3(Random.Range(-_spawnRange, _spawnRange), 3f, Random.Range(-_spawnRange, _spawnRange));
            GameObject gem = Instantiate(_gemPrefab, spawnLocation, _gemPrefab.transform.rotation);
            gemObjects.Add(gem);
        }

        return gemObjects;
    }

    public List<GameObject> SpawnAndGetEnemies(int enemyCount)
    {
        Vector3 spawnLocation;
        List<GameObject> enemyObjects = new List<GameObject>();

        for (int i = 0; i < enemyCount; i++)
        {
            spawnLocation = new Vector3(Random.Range(-_spawnRange, _spawnRange), 3f, Random.Range(-_spawnRange, _spawnRange));
            GameObject enemyObject = Instantiate(_enemyPrefab, spawnLocation, _enemyPrefab.transform.rotation);
            enemyObjects.Add(enemyObject);
        }

        return enemyObjects;
    }

    public void SetSpawnRange(int enemyCount)
    {
        _spawnRange = 4.5f * enemyCount;
    }
}
