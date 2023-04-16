using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Screens _screens;
    [SerializeField] private Level _level;

    private PlayerInput _playerInput;

    private void Awake()
    {
        _screens.GetWinScreen.RestartButtonClicked += OnRestartButtonClicked;
        _screens.GetLooseScreen.RestartButtonClicked += OnRestartButtonClicked;

        _screens.HideAll();
        _level.Init(_screens);
        _level.Won += OnWon;
        _level.Lost += OnLost;
        _playerInput = new PlayerInput();
    }

    //void Start()
    //{
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    //}

    private void OnRestartButtonClicked()
    {
        RestartGame();
    }

    public void RestartGame()
    {
        _playerInput.UI.Restart.performed -= ctx => RestartGame();
        _playerInput.Disable();
        SceneManager.LoadScene(1);
    }

    public void HandleGameOver()
    {
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
        _playerInput.Enable();
        _playerInput.UI.Restart.performed += ctx => RestartGame();
    }

    private void OnWon()
    {
        HandleGameOver();
    }

    private void OnLost()
    {
        HandleGameOver();
    }
}
