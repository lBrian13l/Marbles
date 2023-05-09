using UnityEngine;
using UnityEngine.AI;

public class NavMeshTest : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private GameObject _target;
    private NavMeshPath _path;
    [SerializeField] private Rigidbody _rb;
    public bool PathGenerated;
    [SerializeField] private Enemy _enemy;
    [SerializeField] private NavMeshSurface _surface;

    private void Awake()
    {
        //_surface.BuildNavMesh();

    }

    private void Start()
    {
        _path = new NavMeshPath();
        //_target = FindObjectOfType<PlayerController>().gameObject;
    }

    void Update()
    {
        if (_target != null)
        {
            PathGenerated = NavMesh.CalculatePath(transform.position, _target.transform.position, NavMesh.AllAreas, _path);
        }

        for (int i = 0; i < _path.corners.Length - 1; i++)
            Debug.DrawLine(_path.corners[i], _path.corners[i + 1], Color.red);

        if (_path.corners.Length > 0)
        {
            //_agent.SetPath(_path);
            //_agent.SetDestination(_path.corners[0]);

            _rb.AddForce((_path.corners[1] - transform.position).normalized * Time.deltaTime * 1250);

            _enemy.SetCornerPosition((_path.corners[0] - transform.position).normalized);
        }
    }
}
