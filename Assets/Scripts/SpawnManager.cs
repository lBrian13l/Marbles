using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour, IOnGameOverHandler, IEnemyDiedHandler, IGemCollectedHandler, IPlayerSpawnedHandler
{
    [SerializeField] private GameObject _prefabPowerup;
    [SerializeField] private GameObject _prefabEnemy;
    private float _spawnRange = 45f;
    public int EnemyCount;
    private PlayerController _playerController;
    private int _enemiesOnFirstWave;
    [SerializeField] private GameConfig _gameConfig;
    [SerializeField] private GameObject _plane;
    public List<GameObject> _enemyList = new List<GameObject>();
    public List<GameObject> _gemList = new List<GameObject>();
    private bool _playerSpawned;
    private bool _gameOver;

    private void Awake()
    {
        _enemiesOnFirstWave = _gameConfig.GetEnemyCount();
        if (_enemiesOnFirstWave > 10)
        {
            _plane.transform.localScale = new Vector3(_enemiesOnFirstWave, 1, _enemiesOnFirstWave);
            _spawnRange = 4.5f * _enemiesOnFirstWave;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_gameOver)
        {
            if (_playerSpawned && _enemyList.Count == 0)
            {
                _playerSpawned = false;
                EventBus.RaiseEvent<IOnVictoryHandler>(h => h.HandleOnVictory());
                EventBus.RaiseEvent<IGameOverHandler>(h => h.HandleGameOver());
            }

            if (_playerSpawned && _gemList.Count == 0)
            {
                if (_playerController.PowerupIndicatorIsActive)
                {
                    DestroyEnemy();
                    SpawnPowerup();
                    EventBus.RaiseEvent<IFinishWaveHandler>(h => h.HandleFinishWave());
                }
                else
                {
                    Debug.Log("Game over (no gem)");
                    EventBus.RaiseEvent<IGameOverHandler>(h => h.HandleGameOver());
                }
            }

            EnemyCount = _enemyList.Count;
        }
    }

    public void HandleOnGameOver()
    {
        //gameObject.SetActive(false);
        _gameOver = true;
    }

    void SpawnPowerup()
    {
        Debug.Log(_enemyList.Count);
        Vector3 spawnLocation;
        for (int i = 0; i < _enemyList.Count; i++)
        {
            spawnLocation = new Vector3(Random.Range(-_spawnRange, _spawnRange), 3f, Random.Range(-_spawnRange, _spawnRange));
            GameObject powerup = Instantiate(_prefabPowerup, spawnLocation, _prefabPowerup.transform.rotation);
            _gemList.Add(powerup);
        }
    }

    private void SpawnEnemies(GameObject player)
    {
        Vector3 spawnLocation;
        for (int i = 0; i < _enemiesOnFirstWave; i++)
        {
            spawnLocation = new Vector3(Random.Range(-_spawnRange, _spawnRange), 3f, Random.Range(-_spawnRange, _spawnRange));
            GameObject enemy = Instantiate(_prefabEnemy, spawnLocation, _prefabEnemy.transform.rotation);
            _enemyList.Add(enemy);
            enemy.GetComponent<Enemy>().SetPlayer(player);
        }
    }

    private void DestroyEnemy()
    {
        foreach (GameObject enemy in _enemyList)
        {
            if (!enemy.GetComponent<Enemy>().PowerupIndicatorIsActive)
            {
                _enemyList.Remove(enemy);
                Destroy(enemy);
                return;
            }
        }
    }

    private void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }

    public void HandleEnemyDied(GameObject enemy, bool powerupIsActive)
    {
        _enemyList.Remove(enemy);
        Destroy(enemy);
        if (!powerupIsActive)
        {
            if (_gemList.Count == 1)
            {
                _gemList[0].GetComponent<BoxCollider>().enabled = false;
                EventBus.RaiseEvent<IFinishWaveHandler>(h => h.HandleFinishWave());
                SpawnPowerup();
                Destroy(_gemList[0]);
                _gemList.RemoveAt(0);
            }
            else
            {
                int indexOfGemToDestroy = Random.Range(0, _gemList.Count);
                GameObject gemToDestroy = _gemList[indexOfGemToDestroy];
                _gemList.RemoveAt(indexOfGemToDestroy);
                Destroy(gemToDestroy);
            }
        }
    }

    public void HandleGemCollected(GameObject gem)
    {
        _gemList.Remove(gem);
        Destroy(gem);
    }

    public void HandlePlayerSpawned(GameObject player)
    {
        SpawnEnemies(player);
        SpawnPowerup();
        _playerSpawned = true;
    }
}
