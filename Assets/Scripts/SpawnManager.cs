using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _gemPrefab;
    [SerializeField] private Transform _enemiesRoot;
    [SerializeField] private Transform _gemsRoot;

    public GameObject SpawnAndGetPlayer(Vector3 spawnPoint)
    {
        GameObject playerObject = Instantiate(_playerPrefab, spawnPoint + _playerPrefab.transform.position, _playerPrefab.transform.rotation, transform.parent.parent);
        return playerObject;
    }

    public List<GameObject> SpawnAndGetGems(List<Vector3> spawnPoints)
    {
        List<GameObject> gemObjects = new List<GameObject>();

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            GameObject gem = Instantiate(_gemPrefab, spawnPoints[i] + _gemPrefab.transform.position, _gemPrefab.transform.rotation, _gemsRoot);
            gemObjects.Add(gem);
        }

        return gemObjects;
    }

    public List<GameObject> SpawnAndGetEnemies(List<Vector3> spawnPoints)
    {
        List<GameObject> enemyObjects = new List<GameObject>();

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            GameObject enemyObject = Instantiate(_enemyPrefab, spawnPoints[i] + _enemyPrefab.transform.position, _enemyPrefab.transform.rotation, _enemiesRoot);
            enemyObjects.Add(enemyObject);
        }

        return enemyObjects;
    }
}
