using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour
{
    public GameObject MoveToObject;
    private NavMeshAgent _capsule;

    // Start is called before the first frame update
    void Start()
    {
        _capsule = GetComponent<NavMeshAgent>();
        _capsule.destination = MoveToObject.transform.position;
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
}
