using UnityEngine;
using TMPro;

public class GameScreen : Screen
{
    [SerializeField] private TextMeshProUGUI _enemyCounter;

    public void UpdateEnemyCounter(int enemyCount)
    {
        _enemyCounter.text = $"Enemies: {enemyCount}";
    }
}
