using Meta.XR.MRUtilityKit;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PetSpawner : MonoBehaviour
{
    public enum PetType
    {
        dog,
        cat
    }

    public GameObject spawnedPet;

    [SerializeField] PetType petType;
    [SerializeField] private List<GameObject> petPrefab;
    [SerializeField] private GameObject ball;
    [SerializeField] private Transform spawnLocation;
    [SerializeField] private float minEdgeDistance;
    [SerializeField] private float normalOffset;
    [SerializeField] private MRUKAnchor.SceneLabels spawnLabel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnLocation = Camera.main.transform;
        MRUK.Instance.SceneLoadedEvent.AddListener(() => SpawnPet(petType));
        MRUK.Instance.SceneLoadedEvent.AddListener(SpawnBall);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnBall()
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        LabelFilter labelFilter = new LabelFilter(spawnLabel);

        room.GenerateRandomPositionOnSurface(MRUK.SurfaceType.FACING_UP, minEdgeDistance, labelFilter, out Vector3 surfacePosition, out Vector3 norm);

        Vector3 spawnPos = surfacePosition + norm * normalOffset;
        spawnPos.y = 1;
        Instantiate(ball, spawnPos, Quaternion.identity);
    }

    private void SpawnPet(PetType pet)
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        LabelFilter labelFilter = new LabelFilter(spawnLabel);

        room.GenerateRandomPositionOnSurface(MRUK.SurfaceType.FACING_UP, minEdgeDistance, labelFilter, out Vector3 surfacePosition, out Vector3 norm);
        //room.TryGetClosestSurfacePosition(spawnLocation.position, out Vector3 surfacePosition, out MRUKAnchor closestAnchor, out Vector3 norm, labelFilter);

        Vector3 spawnPos = surfacePosition + norm * normalOffset;
        spawnPos.y = 0;

        switch (pet)
        {
            case PetType.dog:
                spawnedPet = Instantiate(petPrefab[0], spawnPos, Quaternion.identity);
                break;
            case PetType.cat:
                spawnedPet = Instantiate(petPrefab[1], spawnPos, Quaternion.identity);
                break;
            default:
                Debug.Log("pet prefab was spawned");
                break;
        }
    }
}
