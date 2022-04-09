using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IGameOverHandler, IOnVictoryHandler
{
    //public bool gameOver;
    public Button restartButton;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI EnemyCounterText;
    private PlayerInput _playerInput;
    private SpawnManager _spawnManager;
    [SerializeField] private GameObject _playerPrefab;

    private void Awake()
    {
        Instantiate(_playerPrefab, new Vector3(0, 3f, 0), _playerPrefab.transform.rotation);
        _playerInput = new PlayerInput();
        _playerInput.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        _playerInput.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        EnemyCounterText.text = $"Enemies: {_spawnManager.enemyCount}";

        //if (gameOver)
        //{
        //    gameOverText.gameObject.SetActive(true);
        //    restartButton.gameObject.SetActive(true);
        //    _player.Player_Input.Disable();
        //    Cursor.lockState = CursorLockMode.None;
        //    Cursor.visible = true;
        //    _playerInput.Enable();
        //}
    }

    public void RestartGame()
    {
        _playerInput.Disable();
        SceneManager.LoadScene(1);
        _playerInput.Disable();
        //_player.Player_Input.Enable();
    }

    private void OnEnable()
    {
        //_playerInput.Disable();
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }

    public void HandleGameOver()
    {
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _playerInput.Enable();
        _playerInput.UI.Restart.performed += ctx => RestartGame();

        EventBus.RaiseEvent<IOnGameOverHandler>(h => h.HandleOnGameOver());
    }

    public void HandleOnVictory()
    {
        gameOverText.text = "Victory";
    }
}
