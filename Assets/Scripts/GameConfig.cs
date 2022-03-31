using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game Config", order = 51)]
public class GameConfig : ScriptableObject
{
    [SerializeField] private int _enemyCount;
    [SerializeField] private float _rotationSpeed;

    public float GetRotationSpeed()
    {
        return _rotationSpeed;
    }

    public void SetRotationSpeed(float speed)
    {
        _rotationSpeed = speed;
    }

    public int GetEnemyCount()
    {
        return _enemyCount;
    }

    public void SetEnemyCount(int count)
    {
        _enemyCount = count;
    }
}
