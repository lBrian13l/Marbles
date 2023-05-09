using UnityEngine;
using System;

public class Enemy : Character
{
    private Transform _target;
    private Vector3 _nextCornerPosition;

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
            _moveDirection = (_target.position - transform.position).normalized;
            //_moveDirection = (_nextCornerPosition - transform.position).normalized;
        }
    }

    public void SetCornerPosition(Vector3 cornerPosition)
    {
        _nextCornerPosition = cornerPosition;
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
