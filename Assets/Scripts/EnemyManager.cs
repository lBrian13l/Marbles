using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyManager : MonoBehaviour
{
    private List<Enemy> _enemies = new List<Enemy>();

    public event Action EnemiesDied;
    public event Action<int> EnemyCountChanged;
    public event Action NeedToRemoveGem;
    public event Action NeedToTurnOnPlayerIndicator;

    public int EnemyCount => _enemies.Count;

    public void SetEnemies(List<GameObject> enemyObjects, GameObject playerObject)
    {
        foreach (GameObject enemyObject in enemyObjects)
        {
            if (enemyObject.TryGetComponent(out Enemy enemy))
            {
                enemy.EnemyDied += OnEnemyDied;
                enemy.SetPlayer(playerObject);
                _enemies.Add(enemy);
            }
        }
    }

    private void OnEnemyDied(Enemy enemy)
    {
        bool needToRemoveGem = false;

        if (enemy.IndicatorIsActive == false)
        {
            needToRemoveGem = true;
        }

        RemoveEnemy(enemy);

        if (needToRemoveGem)
        {
            NeedToRemoveGem?.Invoke();
        }
    }

    private void RemoveEnemy(Enemy enemy)
    {
        if (_enemies.Contains(enemy))
        {
            enemy.EnemyDied -= OnEnemyDied;
            _enemies.Remove(enemy);
            Destroy(enemy.gameObject);
            EnemyCountChanged?.Invoke(_enemies.Count);
        }

        CheckEnemies();
    }

    public void EnableLastEnemyIndicator()
    {
        foreach (Enemy enemy in _enemies)
        {
            if (enemy.IndicatorIsActive == false)
            {
                enemy.EnableIndicator();
                return;
            }
        }
    }

    public void KillLostEnemy()
    {
        foreach (Enemy enemy in _enemies)
        {
            if (enemy.IndicatorIsActive == false)
            {
                RemoveEnemy(enemy);
                return;
            }
        }
    }

    private void CheckEnemies()
    {
        if (_enemies.Count <= 0)
        {
            EnemiesDied?.Invoke();
        }
    }

    public void DisableIndicators()
    {
        foreach (Enemy enemy in _enemies)
        {
            enemy.DisableIndicator();
        }
    }

    public void HandleGameOver() //временный метод до рефакторинга Enemy
    {
        foreach (Enemy enemy in _enemies)
        {
            enemy.HandleGameOver();
        }
    }
}
