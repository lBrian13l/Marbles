using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour, IOnGameOverHandler, IEnemyDiedHandler, IGemCollectedHandler
{
    public GameObject prefabPowerup;
    public GameObject prefabEnemy;
    float spawnRange = 45f;
    public int enemyCount;
    //int powerupCount;
    //Enemy[] enemies;
    private GameObject _player;
    private PlayerController _playerController;
    //private GameObject[] _powerups;
    //private int _enemiesWithoutIndicator;
    private int _enemiesOnFirstWave;
    [SerializeField] private GameConfig _gameConfig;
    [SerializeField] private GameObject _plane;
    public List<GameObject> _enemyList = new List<GameObject>();
    private List<GameObject> _gemList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player");
        _playerController = _player.GetComponent<PlayerController>();
        _enemiesOnFirstWave = _gameConfig.GetEnemyCount();
        if (_enemiesOnFirstWave > 10)
        {
            _plane.transform.localScale = new Vector3(_enemiesOnFirstWave, 1, _enemiesOnFirstWave);
            spawnRange = 4.5f * _enemiesOnFirstWave;
        }
        SpawnEnemies();
        SpawnPowerup();
    }

    // Update is called once per frame
    void Update()
    {
        //_enemiesWithoutIndicator = 0;
        //foreach (GameObject enemy in _enemyList)
        //{
        //    if (!enemy.GetComponent<Enemy>().PowerupIndicatorIsActive)
        //    {
        //        _enemiesWithoutIndicator++;
        //    }
        //}

        if (_gemList.Count == 0)
        {
            if (_playerController.PowerupIndicatorIsActive)
            {
                SpawnPowerup();
                EventBus.RaiseEvent<IFinishWaveHandler>(h => h.HandleFinishWave());
            }
            else
            {
                Debug.Log("Game over (no gem)");
                EventBus.RaiseEvent<IGameOverHandler>(h => h.HandleGameOver());
            }
        }

        enemyCount = _enemyList.Count;




        //_powerups = GameObject.FindGameObjectsWithTag("Gem");
        //powerupCount = _powerups.Length;
        //enemies = FindObjectsOfType<Enemy>();

        //foreach (Enemy enemy in enemies)
        //{
        //    if (!enemy.transform.Find("Powerup Indicator").gameObject.activeInHierarchy)
        //    {
        //        _enemiesWithoutIndicator++;
        //    }
        //}

        //if (powerupCount > _enemiesWithoutIndicator && !player.transform.Find("Powerup Indicator").gameObject.activeInHierarchy)
        //{
        //    player.transform.Find("Powerup Indicator").gameObject.SetActive(true);
        //    FinishWave();
        //}
        //else if (powerupCount >= _enemiesWithoutIndicator && player.transform.Find("Powerup Indicator").gameObject.activeInHierarchy)
        //{
        //    FinishWave();
        //}

        //if (powerupCount == 0)
        //{
        //    foreach (Enemy enemy in enemies)
        //    {
        //        if (!enemy.transform.Find("Powerup Indicator").gameObject.activeInHierarchy)
        //        {
        //            Destroy(enemy.gameObject);
        //            Debug.Log("Enemy Destroyed");
        //        }
        //        else
        //        {
        //            enemy.transform.Find("Powerup Indicator").gameObject.SetActive(false);
        //            //Debug.Log("Disabled");
        //        }
        //    }

        //    if (player.transform.Find("Powerup Indicator").gameObject.activeInHierarchy)
        //    {
        //        player.transform.Find("Powerup Indicator").gameObject.SetActive(false);
        //        //SpawnPowerup();
        //    }
        //    else
        //    {
        //        //gameManager.gameOver = true;
        //        Debug.Log("Game over (no gem)");
        //        EventBus.RaiseEvent<IGameOverHandler>(h => h.HandleGameOver());
        //    }
        //}

        //if (gameManager.gameOver == true)
        //    gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        //powerupCount = GameObject.FindGameObjectsWithTag("Gem").Length;
        //if (powerupCount == 0)
        //{
        //    SpawnPowerup();
        //}
    }

    public void HandleOnGameOver()
    {
        //gameObject.SetActive(false);
    }

    //private void FinishWave()
    //{
    //    foreach (Enemy enemy in enemies)
    //    {
    //        if (!enemy.transform.Find("Powerup Indicator").gameObject.activeInHierarchy)
    //        {
    //            enemy.transform.Find("Powerup Indicator").gameObject.SetActive(true);
    //        }
    //    }
    //    if (_powerups.Length != 0)
    //    {
    //        foreach (GameObject powerup in _powerups)
    //            Destroy(powerup);
    //    }
    //    powerupCount = 0;
    //}

    void SpawnPowerup()
    {
        //enemyCount = FindObjectsOfType<Enemy>().Length;
        Debug.Log(_enemyList.Count);
        Vector3 spawnLocation;
        for (int i = 0; i < _enemyList.Count; i++)
        {
            spawnLocation = new Vector3(Random.Range(-spawnRange, spawnRange), 3f, Random.Range(-spawnRange, spawnRange));
            GameObject powerup = Instantiate(prefabPowerup, spawnLocation, prefabPowerup.transform.rotation);
            _gemList.Add(powerup);
        }
    }

    void SpawnEnemies()
    {
        Vector3 spawnLocation;
        for (int i = 0; i < _enemiesOnFirstWave; i++)
        {
            spawnLocation = new Vector3(Random.Range(-spawnRange, spawnRange), 3f, Random.Range(-spawnRange, spawnRange));
            GameObject enemy = Instantiate(prefabEnemy, spawnLocation, prefabEnemy.transform.rotation);
            _enemyList.Add(enemy);
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
        if (!powerupIsActive)
        {
            int indexOfGemToDestroy = Random.Range(0, _gemList.Count);
            GameObject gemToDestroy =_gemList[indexOfGemToDestroy];
            _gemList.RemoveAt(indexOfGemToDestroy);
            Destroy(gemToDestroy);
        }
    }

    //private void CleanUpList()
    //{
    //    for (int i = _enemyList.Count - 1; i > -1; i--)
    //    {
    //        if (_enemyList[i] == null)
    //            _enemyList.RemoveAt(i);
    //    }
    //}

    public void HandleGemCollected(GameObject gem)
    {
        _gemList.Remove(gem);
    }
}
