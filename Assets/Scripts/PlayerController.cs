using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IOnGameOverHandler
{
    [SerializeField] float _speed;
    [SerializeField] float _speedLimit;
    private Rigidbody _playerRb;
    private bool _isOnGround;
    [SerializeField] float _jumpForce;
    private GameObject _focalPoint;
    private GameObject _ball;
    private readonly float _ballRadius = 2.5f;
    private Vector3 _steepVector;
    [SerializeField] float _cameraRotationSpeed;
    private Vector3 _normalizedVerticalMovementVector;
    private Vector3 _normalizedMovementVector;
    public PlayerInput Player_Input;
    private Vector2 _moveDirectionInput;
    private Vector2 _lookDirection;
    private float _rotationX;
    private float _rotationY;
    private Quaternion _rotationMovement;
    [SerializeField] float _attackPower;
    private bool _attackedRecently;
    private bool _attackCooldown;
    public float Health;
    private GameObject _powerupIndicator;
    private const float Epsilon = 0.00001f;
    private Vector3 _playerVelocity;
    public float AttackCooldown;
    private AttackCooldownIcon _attackCooldownIcon;

    private void Awake()
    {
        _focalPoint = GameObject.Find("Focal Point");

        Player_Input = new PlayerInput();

        Player_Input.Player.Jump.performed += ctx => OnJump();
        Player_Input.Player.Move.performed += ctx => OnMove();
        Player_Input.Player.Look.performed += ctx => OnLook();
        Player_Input.Player.Attack.performed += ctx => OnAttack();

        //Player_Input.Player.Move1.performed += ctx => OnMove1();
        //Player_Input.Player.Look1.performed += ctx => OnLook1();
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerRb = GetComponent<Rigidbody>();
        _ball = transform.Find("Ball").gameObject;
        _powerupIndicator = transform.Find("Powerup Indicator").gameObject;
        _attackCooldownIcon = FindObjectOfType<AttackCooldownIcon>();
#if UNITY_ANDROID
        _cameraRotationSpeed = 1.5f;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        _moveDirectionInput = Player_Input.Player.Move.ReadValue<Vector2>();
        _lookDirection = Player_Input.Player.Look.ReadValue<Vector2>();

        //_moveDirectionInput = Player_Input.Player.Move1.ReadValue<Vector2>();
        //_lookDirection = Player_Input.Player.Look1.ReadValue<Vector2>();

        Move();
            
        if (!_attackedRecently)
            SpeedLimit();
        RotateBall();

        if (Health <= 0 || transform.position.y < -5)
        {
            EventBus.RaiseEvent<IGameOverHandler>(h => h.HandleGameOver());
        }
    }

    private void LateUpdate()
    {
        RotateCamera();
        _playerVelocity = _playerRb.velocity;
    }

    public void HandleOnGameOver()
    {
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

    //private void OnMove1()
    //{
    //    _moveDirectionInput = Player_Input.Player.Move1.ReadValue<Vector2>();
    //}

    void Move()
    {
        _normalizedMovementVector = new Vector3(_moveDirectionInput.x, 0, _moveDirectionInput.y);
        _normalizedVerticalMovementVector = new Vector3(_focalPoint.transform.forward.x, 0f, _focalPoint.transform.forward.z).normalized;
        _rotationMovement = Quaternion.FromToRotation(Vector3.forward, _normalizedVerticalMovementVector);
        _normalizedMovementVector = _rotationMovement * _normalizedMovementVector;
        _playerRb.AddForce(_normalizedMovementVector * _speed * Time.deltaTime);
    }

    private void OnLook()
    {
        _lookDirection = Player_Input.Player.Look.ReadValue<Vector2>();
    }

    //private void OnLook1()
    //{
    //    _lookDirection = Player_Input.Player.Look1.ReadValue<Vector2>();
    //}

    void RotateCamera()
    {
        _rotationX += _lookDirection.x;
        _rotationY += _lookDirection.y;

        _focalPoint.transform.eulerAngles = new Vector3(-_rotationY * _cameraRotationSpeed, _rotationX * _cameraRotationSpeed, 0);

        if (_rotationY > 50)
            _rotationY = 50;
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

    void RotateBall()
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
        if (other.CompareTag("Gem") && !transform.Find("Powerup Indicator").gameObject.activeInHierarchy)
        {
            Destroy(other.gameObject);
            _powerupIndicator.SetActive(true);
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
                int multiplier = (int)(toEnemyVelocity.magnitude / 10f);
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

    void SpeedLimit()
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
    }

    private void OnDisable()
    {
        Player_Input.Disable();
        EventBus.Unsubscribe(this);
    }
}
