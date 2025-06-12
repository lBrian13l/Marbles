using UnityEngine;
using System;

public class Enemy : Character
{
    private Transform _target;
    private Vector3 _nextCorner;

    public event Action<Enemy> EnemyDied;

    public bool HasTarget => _target ? true : false;
    public Transform Target => _target;

    protected override void Update()
    {
        base.Update();

        if (_target != null)
        {
            CheckDistanceTotarget();
        }
    }

    protected override void SetMoveDirection()
    {
        if (_target)
        {
            Vector3 direction = (_nextCorner - transform.position).normalized;
            direction = new Vector3(direction.x, 0f, direction.z);
            _moveDirection = direction;
        }
        else
        {
            _moveDirection = Vector3.zero;
        }
    }

    public void SetNextCorner(Vector3 corner)
    {
        _nextCorner = corner;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void CheckDistanceTotarget()
    {
        if ((_target.position - transform.position).magnitude < 1)
        {
            _target = null;
        }
    }

    protected override void Die()
    {
        EnemyDied?.Invoke(this);
    }
}
