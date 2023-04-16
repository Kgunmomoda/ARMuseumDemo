using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bridge : MonoBehaviour
{
    public GameObject bridge;
    public NavMeshSurface navMeshSurface;
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        bridge.SetActive(true);
        navMeshSurface.BuildNavMesh();
    }
}
