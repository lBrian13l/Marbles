using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IGameOverHandler, IOnVictoryHandler
{
    [SerializeField] private Button _restartButton;
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private TextMeshProUGUI _enemyCounterText;
    private PlayerInput _playerInput;
    private SpawnManager _spawnManager;
    [SerializeField] private GameObject _playerPrefab;

    private void Awake()
    {
        Instantiate(_playerPrefab, new Vector3(0, 3f, 0), _playerPrefab.transform.rotation);
        _playerInput = new PlayerInput();
    }

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_spawnManager != null)
            _enemyCounterText.text = $"Enemies: {_spawnManager.EnemyCount}";
    }

    public void RestartGame()
    {
            _playerInput.UI.Restart.performed -= ctx => RestartGame();
            _playerInput.Disable();
            SceneManager.LoadScene(1);
    }

    private void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }

    public void HandleGameOver()
    {
        _gameOverText.gameObject.SetActive(true);
        _restartButton.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _playerInput.Enable();
        _playerInput.UI.Restart.performed += ctx => RestartGame();

        EventBus.RaiseEvent<IOnGameOverHandler>(h => h.HandleOnGameOver());
    }

    public void HandleOnVictory()
    {
        _gameOverText.text = "Victory";
    }
}
