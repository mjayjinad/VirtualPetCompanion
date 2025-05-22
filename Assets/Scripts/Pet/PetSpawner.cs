using Meta.XR.MRUtilityKit;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetSpawner : MonoBehaviour
{
    private enum PetType
    {
        dog,
        cat
    }

    public GameObject spawnedPet;

    private PetType petType;
    [SerializeField] private List<GameObject> petPrefab;
    [SerializeField] private GameObject ball;
    [SerializeField] private Button dogBtn;
    [SerializeField] private Button catBtn;
    [SerializeField] private float minEdgeDistance;
    [SerializeField] private float normalOffset;
    [SerializeField] private MRUKAnchor.SceneLabels spawnLabel;
    [SerializeField] private VoiceCommandManager commandManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dogBtn.onClick.AddListener(() => InitializePet(PetType.dog));
        catBtn.onClick.AddListener(() => InitializePet(PetType.cat));
    }

    private void InitializePet(PetType pet)
    {
        SpawnPet(pet);
        SpawnBall();
        commandManager.InitializeSpawnedPet();
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
