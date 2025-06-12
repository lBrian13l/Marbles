using System.Collections.Generic;
using UnityEngine;
using System;

public class Level : MonoBehaviour
{
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private GemManager _gemManager;
    [SerializeField] private AIController _aiController;
    [SerializeField] private NavMeshRandomizer _navMeshRandomizer;
    [SerializeField] private GameConfig _gameConfig;

    private PlayerController _player;
    private Screens _screens;

    public event Action Won;
    public event Action Lost;

    public void Init(Screens screens)
    {
        _gemManager.GemsCollected += OnGemsCollected;
        _gemManager.NeedToEnableIndicator += OnNeedToEnableIndicator;
        _enemyManager.EnemiesDied += OnEnemiesDied;
        _enemyManager.EnemyCountChanged += OnEnemyCountChanged;
        _enemyManager.NeedToRemoveGem += OnNeedToRemoveGem;
        _screens = screens;
        _screens.ShowGameScreen();

        GameObject playerObject = _spawnManager.SpawnAndGetPlayer(_navMeshRandomizer.GetRandomPoints(1)[0]);
        if (playerObject.TryGetComponent(out PlayerController player))
        {
            _player = player;
            _player.PlayerDied += OnPlayerDied;
        }

        int enemyStartCount = _gameConfig.GetEnemyCount();
        _screens.UpdateEnemyCounter(enemyStartCount);

        List<GameObject> gemObjects = _spawnManager.SpawnAndGetGems(_navMeshRandomizer.GetRandomPoints(enemyStartCount));
        _gemManager.SetGems(gemObjects);
        List<GameObject> enemyObjects = _spawnManager.SpawnAndGetEnemies(_navMeshRandomizer.GetRandomPoints(enemyStartCount));
        _enemyManager.SetEnemies(enemyObjects);
        _aiController.SetEnemies(enemyObjects);
        _aiController.SetGems(gemObjects);
        _aiController.SetPlayer(playerObject);
        _aiController.StartSearchCorutine();
    }

    private void OnGemsCollected()
    {
        if (_player.IndicatorIsActive == false)
        {
            Loose();
            return;
        }

        _enemyManager.KillLostEnemy();
        _enemyManager.DisableIndicators();
        _player.DisableIndicator();
        List<GameObject> gemObjects = _spawnManager.SpawnAndGetGems(_navMeshRandomizer.GetRandomPoints(_enemyManager.EnemyCount));
        _gemManager.SetGems(gemObjects);
        _aiController.SetGems(gemObjects);
    }

    private void OnNeedToEnableIndicator()
    {
        if (_player.IndicatorIsActive == false)
        {
            _player.EnableIndicator();
        }
        else
        {
            _enemyManager.EnableLastEnemyIndicator();
        }
    }

    private void OnEnemiesDied()
    {
        Win();
    }

    private void OnEnemyCountChanged(int enemyCount)
    {
        _screens.UpdateEnemyCounter(enemyCount);
    }

    private void OnNeedToRemoveGem()
    {
        _gemManager.RemoveRandomGem();
    }

    private void OnPlayerDied()
    {
        Loose();
    }

    private void Win()
    {
        Debug.Log("Win");
        _screens.ShowWinScreen();
        GameEnded();
        Won?.Invoke();
    }

    private void Loose()
    {
        Debug.Log("Loose");
        _screens.ShowLooseScreen();
        GameEnded();
        Lost?.Invoke();
    }

    private void GameEnded()
    {
        _player.HandleGameOver();
        _enemyManager.HandleGameOver();
        _aiController.StopSearchCorutine();
    }
}
