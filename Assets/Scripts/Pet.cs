using UnityEngine;
using UnityEngine.AI;

public class Pet : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = Camera.main.transform.position;
        agent.SetDestination(targetPosition);
        agent.speed = speed;
    }
}
