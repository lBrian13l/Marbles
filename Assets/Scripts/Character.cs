using UnityEngine;

[RequireComponent(typeof(Ball))]
public abstract class Character : MonoBehaviour
{
    [SerializeField] protected Ball _ball;
    [SerializeField] private GameObject _indicator;

    protected int _health = 100;
    protected Vector3 _moveDirection;
    private Vector3 _lastVelocity;
    private bool _isOnGround;
    private bool _gameOver;
    private const float _epsilon = 0.00001f;

    public bool IndicatorIsActive => _indicator.activeInHierarchy;
    public int Health => _health;

    // Update is called once per frame
    protected virtual void Update()
    {
        if (_gameOver)
            return;

        if (transform.position.y < -5)
        {
            Die();
        }

        SetMoveDirection();
        _ball.Move(_moveDirection);
    }

    protected virtual void LateUpdate()
    {
        _lastVelocity = _ball.Velocity;
    }

    protected void Jump()
    {
        if (_isOnGround)
        {
            _ball.Jump();
        }
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;

        if (_health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {

    }

    protected virtual void SetMoveDirection()
    {

    }

    public virtual void HandleGameOver()
    {
        _gameOver = true;
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
            Vector3 toEnemyVelocity = Vector3.Project(_lastVelocity, toEnemyVector);
            if (AreCodirected(toEnemyVector, toEnemyVelocity))
            {
                int multiplier = (int)(toEnemyVelocity.magnitude / 10f);
                collision.gameObject.GetComponent<Character>().TakeDamage(10 * multiplier);
                //Debug.Log($"Speed: {toEnemyVelocity.magnitude}, multiplier: {multiplier}");
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isOnGround = false;
        }
    }

    private bool AreCodirected(Vector3 vector1, Vector3 vector2)
    {
        return Vector3.Dot(vector1.normalized, vector2.normalized) > 1 - _epsilon;
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
