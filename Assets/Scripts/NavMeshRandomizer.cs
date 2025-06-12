using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshRandomizer : MonoBehaviour
{
    [SerializeField] private float _radius = 40f;

    private int _spawnableMask => GetSpawnableMask();

    public List<Vector3> GetRandomPoints(int count)
    {
        List<Vector3> randomPoints = new List<Vector3>();

        for (int i = 0; i < count; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * _radius;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2.5f, _spawnableMask))
            {
                randomPoint = hit.position;
                randomPoints.Add(randomPoint);
            }
            else
            {
                i--;
            }
        }

        return randomPoints;
    }

    private int GetSpawnableMask()
    {
        int mask = 0;
        mask += 1 << NavMesh.GetAreaFromName("Walkable");

        return mask;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawSphere(transform.position, _radius);
    }
}
