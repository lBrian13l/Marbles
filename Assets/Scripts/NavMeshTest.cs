using UnityEngine;
using UnityEngine.AI;

public class NavMeshTest : MonoBehaviour
{
    private NavMeshAgent _agent;
    private GameObject _player;
    private NavMeshPath _path;
    private Rigidbody _rb;
    public bool PathGenerated;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _path = new NavMeshPath();
        _rb = GetComponent<Rigidbody>();

    }

    void Update()
    {
        if (_player == null)
            _player = FindObjectOfType<PlayerController>().gameObject;

        if (_player != null)
        {
            PathGenerated = NavMesh.CalculatePath(transform.position, _player.transform.position,NavMesh.AllAreas, _path);
        }

        for (int i = 0; i < _path.corners.Length - 1; i++)
            Debug.DrawLine(_path.corners[i], _path.corners[i + 1], Color.red);

        if (_path.corners.Length != 0)
        {
            _agent.SetPath(_path);
            _agent.SetDestination(_path.corners[0]);

            _rb.AddForce((_path.corners[0] - transform.position).normalized * Time.deltaTime * 100);
        }
    }
}
