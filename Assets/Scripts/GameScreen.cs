using UnityEngine;
using TMPro;

public class GameScreen : Screen
{
    [SerializeField] private TextMeshProUGUI _enemyCounter;
    [SerializeField] private GameObject _mobileUIRoot;

    public void UpdateEnemyCounter(int enemyCount)
    {
        _enemyCounter.text = $"Enemies: {enemyCount}";
    }
#if PLATFORM_ANDROID
    private void OnEnable()
    {
        _mobileUIRoot.SetActive(true);
    }
#endif
}
