using System.Collections;
using UnityEngine;
using System;

public class PlayerController : Character
{
    //TODO refactoring

    [SerializeField] private GameObject _focalPoint;
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
    private bool _attackCooldown;
    public float AttackCooldown;
    private AttackCooldownIcon _attackCooldownIcon;
    [SerializeField] private GameConfig _gameConfig;
    [SerializeField] private GameObject _camera;
    private float _cameraZoom = 12f;
    private Vector3 _cameraPos;
    private float _zoomingSpeed = 10f;
    private bool _isMoving;
    private bool _isRotating;
    //private NavMeshAgent _navMeshAgent;

    public event Action PlayerDied;

    private void Awake()
    {
        SubscribeInput();
    }

    void Start()
    {
        //_navMeshAgent = GetComponent<NavMeshAgent>();
        _attackCooldownIcon = FindObjectOfType<AttackCooldownIcon>();
        _cameraRotationSpeed = _gameConfig.GetRotationSpeed();
        _cameraPos = new Vector3(0f, _cameraZoom, -_cameraZoom);
    }

    protected override void Update()
    {
        base.Update();

        _moveDirectionInput = Player_Input.Player.Move.ReadValue<Vector2>();
        _lookDirection = Player_Input.Player.Look.ReadValue<Vector2>();

        //if (NavMesh.SamplePosition(_navMeshAgent.transform.position, out NavMeshHit hit, 2.25f, NavMesh.AllAreas))
        //{
        //    if (hit.mask == 8)
        //    {
        //        _navMeshAgent.enabled = false;
        //    }
        //}
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        RotateCamera();
        Zooming();
    }

    public override void HandleGameOver()
    {
        base.HandleGameOver();

        enabled = false;
        GetComponent<SphereCollider>().enabled = false;
        DisableIndicator();
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponentInChildren<Renderer>().enabled = false;
        Player_Input.Disable();
        _health = 0;
    }

    public void OnAttack()
    {
        if (!_attackCooldown)
        {
            _attackCooldown = true;
            _normalizedVerticalMovementVector = new Vector3(_focalPoint.transform.forward.x, 0f, _focalPoint.transform.forward.z).normalized;
            _normalizedVerticalMovementVector *= _attackPower;
            _ball.Attack(_normalizedVerticalMovementVector);
            _attackCooldownIcon.Attacked();
            StartCoroutine(c_AttackCooldown());
        }
    }

    private IEnumerator c_AttackCooldown()
    {
        yield return new WaitForSeconds(AttackCooldown);
        _attackCooldown = false;
    }

    private void OnMove()
    {
        _moveDirectionInput = Player_Input.Player.Move.ReadValue<Vector2>();
    }

    protected override void SetMoveDirection()
    {
        _normalizedMovementVector = new Vector3(_moveDirectionInput.x, 0, _moveDirectionInput.y);
        _normalizedVerticalMovementVector = new Vector3(_focalPoint.transform.forward.x, 0f, _focalPoint.transform.forward.z).normalized;
        _rotationMovement = Quaternion.FromToRotation(Vector3.forward, _normalizedVerticalMovementVector);
        _normalizedMovementVector = _rotationMovement * _normalizedMovementVector;
        _moveDirection = _normalizedMovementVector;
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
        StartCoroutine(c_ZoomDetection());
    }

    private void ZoomEnd()
    {
        StopCoroutine(c_ZoomDetection());
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
        Jump();
    }

    protected override void Die()
    {
        PlayerDied?.Invoke();
    }

    private void OnEnable()
    {
        Player_Input.Enable();
    }

    private void OnDisable()
    {
        Player_Input.Disable();
    }

    private void SubscribeInput()
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
}
