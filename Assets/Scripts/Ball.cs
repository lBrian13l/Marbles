using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private GameObject _visibleBall;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _speedLimit;
    [SerializeField] private float _jumpForce = 50;

    private Vector3 _steepVector;
    private float _ballRadius = 2.5f;

    public Vector3 Velocity => _rigidbody.velocity;

    public void Move(Vector3 direction)
    {
        _rigidbody.AddForce(direction.normalized * _moveSpeed * Time.deltaTime, ForceMode.Force);
        LimitSpeed();
        RotateVisibleBall();
    }

    private void LimitSpeed()
    {
        if (_rigidbody.velocity.magnitude > _speedLimit)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * _speedLimit;
        }
    }

    private void RotateVisibleBall()
    {
        Vector3 movement = _rigidbody.velocity * Time.deltaTime;
        Vector3 rotationAxis = Vector3.Cross(Vector3.up + _steepVector, movement).normalized;
        float distance = movement.magnitude;
        float angle = distance * (180 / Mathf.PI) / _ballRadius;
        _visibleBall.transform.Rotate(rotationAxis * angle, Space.World);
    }

    public void Jump()
    {
        _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }

    public void Attack(Vector3 direction)
    {
        _rigidbody.AddForce(direction, ForceMode.Impulse);
        StartCoroutine(ChangeSpeedLimitForSeconds(999, 0.5f));
    }

    private IEnumerator ChangeSpeedLimitForSeconds(float newSpeedLimit, float time)
    {
        float oldSpeedLimit = _speedLimit;
        _speedLimit = newSpeedLimit;
        yield return new WaitForSeconds(time);
        _speedLimit = oldSpeedLimit;
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

    //private void OnCollisionExit(Collision collision)
    //{
    //    _steepVector = Vector3.zero;
    //}
}
