using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] private float _searchDelay = 1;

    private List<Enemy> _enemies = new List<Enemy>();
    private List<GameObject> _gemObjects;
    private GameObject _playerObject;
    private Coroutine _targetSearchCorutine;

    private IEnumerator SearchTarget()
    {
        while (true)
        {
            foreach (Enemy enemy in _enemies)
            {
                if (enemy == null)
                    continue;

                if (enemy.IndicatorIsActive)
                {
                    if (enemy.HasTarget == false)
                    {
                        enemy.SetTarget(_playerObject.transform);
                    }
                }
                else if (enemy.HasTarget == false || enemy.Target == _playerObject.transform)
                {
                    Transform target = FindNearestGem(enemy.gameObject);
                    enemy.SetTarget(target);
                }
            }

            yield return new WaitForSeconds(_searchDelay);
        }
    }

    private Transform FindNearestGem(GameObject enemy)
    {
        Transform nearestGem = null;
        float distanceToGem = 9999f;

        foreach (GameObject gem in _gemObjects)
        {
            if (gem == null)
                continue;

            if ((gem.transform.position - enemy.transform.position).magnitude < distanceToGem)
            {
                distanceToGem = (gem.transform.position - enemy.transform.position).magnitude;
                nearestGem = gem.transform;
            }
        }

        return nearestGem;
    }

    public void StartSearchCorutine()
    {
        _targetSearchCorutine = StartCoroutine(SearchTarget());
    }

    public void StopSearchCorutine()
    {
        if (_targetSearchCorutine != null)
        {
            StopCoroutine(_targetSearchCorutine);
        }
    }

    public void SetEnemies(List<GameObject> enemyObjects)
    {
        foreach (GameObject enemyObject in enemyObjects)
        {
            if (enemyObject.TryGetComponent(out Enemy enemy))
            {
                _enemies.Add(enemy);
            }
        }
    }

    public void SetGems(List<GameObject> gemObjects)
    {
        _gemObjects = gemObjects;
    }

    public void SetPlayer(GameObject playerObject)
    {
        _playerObject = playerObject;
    }
}
