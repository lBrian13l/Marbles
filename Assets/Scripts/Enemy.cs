using UnityEngine;
using UnityEngine.AI;
using System;

public class Enemy : MonoBehaviour
{
    //TODO отрефакторить прошлогодний скрипт

    private GameObject[] _gems;
    private Rigidbody _enemyRb;
    [SerializeField] private float _speed;
    private GameObject _player;
    private bool _isOnGround;
    private Vector3 _steepVector;
    private readonly float _ballRadius = 2.5f;
    [SerializeField] private GameObject _ball;
    public float Health;
    private Vector3 _enemyVelocity;
    private const float Epsilon = 0.00001f;
    private bool _gameOver;
    [SerializeField] private GameObject _indicator;
    public bool IndicatorIsActive => _indicator.activeInHierarchy;
    [SerializeField] private float _speedLimit;
    private NavMeshAgent _navMeshAgent;

    public event Action<Enemy> EnemyDied;

    void Start()
    {
        _enemyRb = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (transform.position.y < -5 || Health <= 0)
        {
            EnemyDied?.Invoke(this);
        }

        if (!_gameOver)
        {
            Move();
            SpeedLimit();
        }
        RotateBall();

        if (NavMesh.SamplePosition(_navMeshAgent.transform.position, out NavMeshHit hit, 2.25f, NavMesh.AllAreas))
        {
            Debug.Log(hit.mask);
            if (hit.mask == 8)
            {
                _navMeshAgent.enabled = false;
            }
        }
    }

    private void LateUpdate()
    {
        _enemyVelocity = _enemyRb.velocity;
    }

    void Move()
    {
        if (!IndicatorIsActive && _isOnGround)
        {
            MoveToPowerup();
        }
        else if (_isOnGround)
        {
            MoveToPlayer();
        }
    }

    void MoveToPowerup()
    {
        _gems = GameObject.FindGameObjectsWithTag("Gem");
        float distanceToPowerup = 999f;
        if (_gems.Length == 0)
        {

        }
        else
        {
            Vector3 toPowerup = Vector3.zero;
            foreach (GameObject gem in _gems)
            {
                if ((gem.transform.position - transform.position).magnitude < distanceToPowerup)
                {
                    distanceToPowerup = (gem.transform.position - transform.position).magnitude;
                    toPowerup = (gem.transform.position - transform.position).normalized;
                }
            }
            _enemyRb.AddForce(toPowerup * _speed * Time.deltaTime, ForceMode.Force);
        }
    }

    void MoveToPlayer()
    {
        _enemyRb.AddForce((_player.transform.position - transform.position).normalized * _speed * Time.deltaTime, ForceMode.Force);
    }

    void RotateBall()
    {
        Vector3 movement = _enemyRb.velocity * Time.deltaTime;
        Vector3 rotationAxis = Vector3.Cross(Vector3.up + _steepVector, movement).normalized;
        float distance = movement.magnitude;
        float angle = distance * (180 / Mathf.PI) / _ballRadius;
        _ball.transform.Rotate(rotationAxis * angle, Space.World);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isOnGround = true;
        }

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            Vector3 toEnemyVector = collision.transform.position - transform.position;
            Vector3 toEnemyVelocity = Vector3.Project(_enemyVelocity, toEnemyVector);
            if (AreCodirected(toEnemyVector, toEnemyVelocity))
            {
                int multiplier = (int)(toEnemyVelocity.magnitude / 10f);
                if (collision.gameObject.CompareTag("Enemy"))
                {
                    collision.gameObject.GetComponent<Enemy>().Health -= 10 * multiplier;
                }
                else
                {
                    _player.GetComponent<PlayerController>().Health -= 10 * multiplier;
                }
                //Debug.Log($"Speed: {toEnemyVelocity.magnitude}, multiplier: {multiplier}");
            }
        }
    }

    private bool AreCodirected(Vector3 vector1, Vector3 vector2)
    {
        return Vector3.Dot(vector1.normalized, vector2.normalized) > 1 - Epsilon;
    }

    private void OnCollisionExit(Collision collision)
    {
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

    public void HandleGameOver()
    {
        _gameOver = true;
    }

    public void SetPlayer(GameObject player)
    {
        _player = player;
    }

    void SpeedLimit()
    {
        if (_enemyRb.velocity.magnitude > _speedLimit)
        {
            _enemyRb.velocity = _enemyRb.velocity.normalized * _speedLimit;
        }
    }

    public void DisableIndicator()
    {
        _indicator.SetActive(false);
    }

    public void EnableIndicator()
    {
        _indicator.SetActive(true);
    }
}
