using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screens : MonoBehaviour
{
    [SerializeField] private GameScreen _gameScreen;
    [SerializeField] private WinScreen _winScreen;
    [SerializeField] private LooseScreen _looseScreen;

    public GameScreen GetGameScreen => _gameScreen;
    public WinScreen GetWinScreen => _winScreen;
    public LooseScreen GetLooseScreen => _looseScreen;

    public void UpdateEnemyCounter(int enemyCount)
    {
        _gameScreen.UpdateEnemyCounter(enemyCount);
    }

    public void HideAll()
    {
        _gameScreen.Hide();
        _winScreen.Hide();
        _looseScreen.Hide();
    }

    public void ShowGameScreen()
    {
        _gameScreen.Show();
    }

    public void ShowWinScreen()
    {
        _gameScreen.Hide();
        _winScreen.Show();
    }

    public void ShowLooseScreen()
    {
        _gameScreen.Hide();
        _looseScreen.Show();
    }
}
