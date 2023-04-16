using UnityEngine;
using UnityEngine.UI;
using System;

public abstract class EndScreen : Screen
{
    [SerializeField] private Button _restartButton;

    public event Action RestartButtonClicked;

    private void OnEnable()
    {
        _restartButton.onClick.AddListener(delegate { RestartButtonClicked?.Invoke(); });
    }

    private void OnDisable()
    {
        _restartButton.onClick.RemoveAllListeners();
    }
}
