//using System.Collections;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Pet : MonoBehaviour
{
    enum PetState 
    { 
        Idle, 
        GoingToBall, 
        ReturningBall,
        Wiggle
    }
    PetState currentState = PetState.Idle;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform ballParent;
    [SerializeField] private float speed;
    [SerializeField] private float stopDistanceToPlayer;
    [SerializeField] private float stopDistanceToBall;
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 4f;

    // Distance thresholds
    public float runDistanceThreshold = 3.5f;
    private GameObject ball;
    private Transform userTransform;

    void Start()
    {
        userTransform = Camera.main.transform;
        agent.speed = speed;
    }

    void Update()
    {
        switch (currentState)
        {
            case PetState.Idle:
                // Follow user
                agent.SetDestination(userTransform.position);
                float distToUser = Vector3.Distance(transform.position, userTransform.position);
                if (distToUser < stopDistanceToPlayer)
                {
                    animator.SetInteger("AnimationID", 7);  // Sit idle animation
                    agent.isStopped = true;
                }
                else
                {
                    //animator.SetInteger("AnimationID", 3);
                    //agent.isStopped = false;
                }
                break;

            case PetState.GoingToBall:
                if (ball != null)
                {
                    animator.SetInteger("AnimationID", 3);
                    agent.isStopped = false;
                    agent.SetDestination(ball.transform.position);
                    // Check distance to ball to pick it up
                    if (Vector3.Distance(transform.position, ball.transform.position) < stopDistanceToBall)
                    {
                        PickUpBall();
                    }
                }
                else
                {
                    currentState = PetState.Idle; // ball lost, back to idle
                }
                break;

            case PetState.ReturningBall:
                animator.SetInteger("AnimationID", 3);
                agent.isStopped = false;
                agent.SetDestination(userTransform.position);
                // Check if near user to drop ball
                if (Vector3.Distance(transform.position, userTransform.position) < stopDistanceToPlayer)
                {
                    DropBall();
                }
                break;
            case PetState.Wiggle:
                Wiggle();
                break;
        }
    }

    public void StartWiggle()
    {
        currentState = PetState.Wiggle;
    }
    
    private void Wiggle()
    {
        StartCoroutine(TouchPet());
    }

    private IEnumerator TouchPet()
    {
        animator.SetInteger("AnimationID", 1);

        yield return new WaitForSeconds(3);

        animator.SetInteger("AnimationID", 7);
        currentState = PetState.Idle;
    }

    public void StartFetch(GameObject thrownBall)
    {
        ball = thrownBall;
        currentState = PetState.GoingToBall;
    }

    void PickUpBall()
    {
        agent.isStopped = true;
        animator.SetInteger("AnimationID", 5);

        // Attach ball to pet
        ball.transform.localPosition = ballParent.position; // adjust as needed
        ball.transform.SetParent(ballParent);
        ball.GetComponent<Rigidbody>().isKinematic = true;

        // Disable NavMeshAgent automatic rotation so we can control rotation manually
        agent.updateRotation = false;

        agent.angularSpeed = 120f;
        // Start smooth rotation coroutine toward the player
        StartCoroutine(SmoothTurnTowards(userTransform.position, 1f));

        // After turning completes, change state and resume agent
        StartCoroutine(ResumeAfterTurn());
    }

    private IEnumerator ResumeAfterTurn()
    {
        // Wait for the turn duration
        yield return new WaitForSeconds(1f);

        currentState = PetState.ReturningBall;
        agent.isStopped = false;

        // NavMeshAgent rotation re-enabled inside SmoothTurnTowards coroutine after turn ends
    }

    private IEnumerator SmoothTurnTowards(Vector3 targetPosition, float duration)
    {
        Quaternion startRot = transform.rotation;
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion endRot = Quaternion.LookRotation(direction);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRot;

        // Re-enable NavMeshAgent rotation after smooth turn
        agent.updateRotation = true;
    }

    void DropBall()
    {
        // Detach ball
        ball.transform.SetParent(null);
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;

        ball = null;
        currentState = PetState.Idle;
    }
}