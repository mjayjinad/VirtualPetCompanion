using Meta.XR.MRUtilityKit;
using UnityEngine;
using Unity.AI.Navigation;
using System.Collections;

public class RuntimeNavmeshBuilder : MonoBehaviour
{
    private NavMeshSurface navMeshSurface;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
        MRUK.Instance.RegisterSceneLoadedCallback(BuildNavmesh);
    }

    private void BuildNavmesh()
    {
        StartCoroutine(BuildNavmeshCoroutine());
    }

    private IEnumerator BuildNavmeshCoroutine()
    {
        yield return new WaitForEndOfFrame();
        navMeshSurface.BuildNavMesh();
    }
}
