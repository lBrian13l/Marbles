using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IOnGameOverHandler, IFinishWaveHandler
{
    [SerializeField] private float _speed;
    [SerializeField] private float _speedLimit;
    private Rigidbody _playerRb;
    private bool _isOnGround;
    [SerializeField] private float _jumpForce;
    [SerializeField] private GameObject _focalPoint;
    [SerializeField] private GameObject _ball;
    private readonly float _ballRadius = 2.5f;
    private Vector3 _steepVector;
    private float _cameraRotationSpeed;
    private Vector3 _normalizedVerticalMovementVector;
    private Vector3 _normalizedMovementVector;
    public PlayerInput Player_Input;
    private Vector2 _moveDirectionInput;
    private Vector2 _lookDirection;
    private float _rotationX;
    private float _rotationY;
    private Quaternion _rotationMovement;
    [SerializeField] private float _attackPower;
    private bool _attackedRecently;
    private bool _attackCooldown;
    public float Health;
    [SerializeField] private GameObject _powerupIndicator;
    public bool PowerupIndicatorIsActive;
    private const float Epsilon = 0.00001f;
    private Vector3 _playerVelocity;
    public float AttackCooldown;
    private AttackCooldownIcon _attackCooldownIcon;
    [SerializeField] private GameConfig _gameConfig;
    [SerializeField] private GameObject _camera;
    private float _cameraZoom = 12f;
    private Vector3 _cameraPos;
    private float _zoomingSpeed = 10f;
    private bool _isMoving;
    private bool _isRotating;
    private bool _gameOver;

    private void Awake()
    {
        Player_Input = new PlayerInput();

        Player_Input.Player.Jump.performed += ctx => OnJump();
        Player_Input.Player.Move.performed += ctx => OnMove();
        Player_Input.Player.Move.started += ctx => OnMoveStart();
        Player_Input.Player.Move.canceled += ctx => OnMoveEnd();
        Player_Input.Player.Look.performed += ctx => OnLook();
        Player_Input.Player.Look.started += ctx => OnLookStart();
        Player_Input.Player.Look.canceled += ctx => OnLookEnd();
        Player_Input.Player.Attack.performed += ctx => OnAttack();
        Player_Input.Player.Zoom.performed += ctx => OnZoom();
        Player_Input.Player.SecondaryTouchContact.started += ctx => ZoomStart();
        Player_Input.Player.SecondaryTouchContact.canceled += ctx => ZoomEnd();
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerRb = GetComponent<Rigidbody>();
        _attackCooldownIcon = FindObjectOfType<AttackCooldownIcon>();
        _cameraRotationSpeed = _gameConfig.GetRotationSpeed();
        _cameraPos = new Vector3(0f, _cameraZoom, -_cameraZoom);
    }

    // Update is called once per frame
    void Update()
    {
        _moveDirectionInput = Player_Input.Player.Move.ReadValue<Vector2>();
        _lookDirection = Player_Input.Player.Look.ReadValue<Vector2>();

        Move();
            
        if (!_attackedRecently)
            SpeedLimit();
        RotateBall();

        if (!_gameOver)
        {
            if (Health <= 0 || transform.position.y < -5)
            {
                EventBus.RaiseEvent<IGameOverHandler>(h => h.HandleGameOver());
            }
        }
    }

    private void LateUpdate()
    {
        RotateCamera();
        _playerVelocity = _playerRb.velocity;
        Zooming();
    }

    public void HandleOnGameOver()
    {
        _gameOver = true;
        GetComponent<SphereCollider>().enabled = false;
        _powerupIndicator.SetActive(false);
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        _ball.GetComponent<Renderer>().enabled = false;
        Player_Input.Disable();
        Health = 0;
    }

    public void OnAttack()
    {
        if (!_attackCooldown)
        {
            _attackedRecently = true;
            _attackCooldown = true;
            _normalizedVerticalMovementVector = new Vector3(_focalPoint.transform.forward.x, 0f, _focalPoint.transform.forward.z).normalized;
            _playerRb.AddForce(_normalizedVerticalMovementVector * _attackPower, ForceMode.Impulse);
            _attackCooldownIcon.Attacked();
            StartCoroutine("c_AttackCooldown");
            StartCoroutine("c_SpeedLimitDisabled");
        }
    }

    IEnumerator c_AttackCooldown()
    {
        yield return new WaitForSeconds(AttackCooldown);
        _attackCooldown = false;
    }

    IEnumerator c_SpeedLimitDisabled()
    {
        yield return new WaitForSeconds(0.5f);
        _attackedRecently = false;
    }

    private void OnMove()
    {
        _moveDirectionInput = Player_Input.Player.Move.ReadValue<Vector2>();
    }

    private void Move()
    {
        _normalizedMovementVector = new Vector3(_moveDirectionInput.x, 0, _moveDirectionInput.y);
        _normalizedVerticalMovementVector = new Vector3(_focalPoint.transform.forward.x, 0f, _focalPoint.transform.forward.z).normalized;
        _rotationMovement = Quaternion.FromToRotation(Vector3.forward, _normalizedVerticalMovementVector);
        _normalizedMovementVector = _rotationMovement * _normalizedMovementVector;
        _playerRb.AddForce(_normalizedMovementVector * _speed * Time.deltaTime);
    }

    private void OnMoveStart()
    {
        _isMoving = true;
    }

    private void OnMoveEnd()
    {
        _isMoving = false;
    }

    private void OnLookStart()
    {
        _isRotating = true;
    }

    private void OnLookEnd()
    {
        _isRotating = false;
    }

    private void OnLook()
    {
        _lookDirection = Player_Input.Player.Look.ReadValue<Vector2>();
    }

    private void OnZoom()
    {
        _cameraZoom -= Player_Input.Player.Zoom.ReadValue<Vector2>().y;

        if (_cameraZoom > 40)
            _cameraZoom = 40;
        else if (_cameraZoom < 7)
            _cameraZoom = 7;

        _cameraPos = new Vector3(0f, _cameraZoom, -_cameraZoom);
    }

    private void Zooming()
    {
        _camera.transform.localPosition = Vector3.Lerp(_camera.transform.localPosition, _cameraPos, Time.deltaTime * _zoomingSpeed);
    }

    private void ZoomStart()
    {
        StartCoroutine("c_ZoomDetection");
    }

    private void ZoomEnd()
    {
        StopCoroutine("c_ZoomDetection");
    }

    private IEnumerator c_ZoomDetection()
    {
        float previousDistance = 0f;
        float distance;
        while (!_isMoving && !_isRotating)
        {
            distance = Vector2.Distance(Player_Input.Player.PrimaryFingerPos.ReadValue<Vector2>(), Player_Input.Player.SecondaryFingerPos.ReadValue<Vector2>());
            if (distance > previousDistance)
            {
                _cameraZoom -= 0.25f;

                if (_cameraZoom < 7)
                    _cameraZoom = 7;
            }
            else if (distance < previousDistance)
            {
                _cameraZoom += 0.25f;

                if (_cameraZoom > 40)
                    _cameraZoom = 40;
            }
            _cameraPos = new Vector3(0f, _cameraZoom, -_cameraZoom);
            previousDistance = distance;
            yield return null;
        }
    }
    
    private void RotateCamera()
    {
        _rotationX += _lookDirection.x;
        _rotationY += _lookDirection.y;

        _focalPoint.transform.eulerAngles = new Vector3(-_rotationY * _cameraRotationSpeed, _rotationX * _cameraRotationSpeed, 0);

        if (_rotationY > 30)
            _rotationY = 30;
        else if (_rotationY < -35)
            _rotationY = -35;
    }

    public void OnJump()
    {
        if (_isOnGround)
        {
            _playerRb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }

    private void RotateBall()
    {
        Vector3 movement = _playerRb.velocity * Time.deltaTime;
        Vector3 rotationAxis = Vector3.Cross(Vector3.up + _steepVector, movement).normalized;
        movement -= (Vector3.up + _steepVector) * Vector3.Dot(movement, (Vector3.up + _steepVector));
        float distance = movement.magnitude;
        float angle = distance * (180 / Mathf.PI) / _ballRadius;
        _ball.transform.Rotate(rotationAxis * angle, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gem") && !PowerupIndicatorIsActive)
        {
            _powerupIndicator.SetActive(true);
            PowerupIndicatorIsActive = true;
            EventBus.RaiseEvent<IGemCollectedHandler>(h => h.HandleGemCollected(other.gameObject));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isOnGround = true;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector3 toEnemyVector = collision.transform.position - transform.position;
            Vector3 toEnemyVelocity = Vector3.Project(_playerVelocity, toEnemyVector);
            if (AreCodirected(toEnemyVector, toEnemyVelocity))
            {
                int multiplier = (int)(toEnemyVelocity.magnitude / 15f);
                Damage(collision, multiplier);
                //Debug.Log($"Speed: {toEnemyVelocity.magnitude}, multiplier: {multiplier}");
            }
        }
    }

    private bool AreCodirected(Vector3 vector1, Vector3 vector2)
    {
        return Vector3.Dot(vector1.normalized, vector2.normalized) > 1 - Epsilon;
    }

    private void Damage(Collision collision, int multiplier)
    {
        collision.gameObject.GetComponent<Enemy>().Health -= 10 * multiplier;
    }

    private void OnCollisionExit(Collision collision)
    {
        _steepVector = Vector3.zero;
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isOnGround = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        _steepVector = Vector3.zero;
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (!collision.gameObject.CompareTag("Ground"))
            {
                _steepVector += collision.GetContact(i).normal;
            }
        }
    }

    private void SpeedLimit()
    {
        if (_playerRb.velocity.magnitude > _speedLimit)
        {
            _playerRb.velocity = _playerRb.velocity.normalized * _speedLimit;
        }
    }

    private void OnEnable()
    {
        Player_Input.Enable();
        EventBus.Subscribe(this);
        EventBus.RaiseEvent<IPlayerSpawnedHandler>(h => h.HandlePlayerSpawned(transform.gameObject));
    }

    private void OnDisable()
    {
        Player_Input.Disable();
        EventBus.Unsubscribe(this);
    }

    public void HandleFinishWave()
    {
        _powerupIndicator.SetActive(false);
        PowerupIndicatorIsActive = false;
    }
}
