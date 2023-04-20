using UnityEngine;
using System;

public class Enemy : Character
{
    private Transform _target;
    //private NavMeshAgent _navMeshAgent;

    public event Action<Enemy> EnemyDied;

    public bool HasTarget => _target ? true : false;
    public Transform Target => _target;

    //void Start()
    //{
    //    _navMeshAgent = GetComponent<NavMeshAgent>();
    //}

    protected override void Update()
    {
        base.Update();

        if (_target != null)
        {
            CheckDistanceTotarget();
        }

        //if (NavMesh.SamplePosition(_navMeshAgent.transform.position, out NavMeshHit hit, 2.25f, NavMesh.AllAreas))
        //{
        //    Debug.Log(hit.mask);
        //    if (hit.mask == 8)
        //    {
        //        _navMeshAgent.enabled = false;
        //    }
        //}
    }

    protected override void SetMoveDirection()
    {
        if (_target)
        {
            _moveDirection = (_target.position - transform.position).normalized;
        }
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
