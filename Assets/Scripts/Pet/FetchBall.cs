using Oculus.Interaction;
using UnityEngine;

public class FetchBall : MonoBehaviour
{
    public Pet pet;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pet = FindAnyObjectByType<Pet>();
    }

    public void StartFetch()
    {
        pet.StartFetch(gameObject);
    }
}
