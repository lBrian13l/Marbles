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
        PlayerPrefs.SetFloat("RotationSpeed", _rotationSpeed);
        PlayerPrefs.Save();
    }

    public int GetEnemyCount()
    {
        return _enemyCount;
    }

    public void SetEnemyCount(int count)
    {
        _enemyCount = count;
        PlayerPrefs.SetInt("EnemyCount", _enemyCount);
        PlayerPrefs.Save();
    }

    private void Awake()
    {
        if (PlayerPrefs.HasKey("EnemyCount"))
            _enemyCount = PlayerPrefs.GetInt("EnemyCount");

        if (PlayerPrefs.HasKey("RotationSpeed"))
            _rotationSpeed = PlayerPrefs.GetFloat("RotationSpeed");
    }
}
