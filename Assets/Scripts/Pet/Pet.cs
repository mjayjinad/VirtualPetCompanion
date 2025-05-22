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
        Wiggle,
        Bark,
        Stand,
        Sit,
        WalkToPlayer,
        FollowPlayer,
        Fetch
    }

    PetState currentState = PetState.Idle;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform ballParent;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip barkClip;
    [SerializeField] private float speed;
    [SerializeField] private float stopDistanceToPlayer;
    [SerializeField] private float stopDistanceToBall;

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
                agent.SetDestination(userTransform.position);
                if (Vector3.Distance(transform.position, userTransform.position) < stopDistanceToPlayer)
                {
                    animator.SetInteger("AnimationID", 0);
                    agent.isStopped = true;
                }
                break;

            case PetState.GoingToBall:
                animator.SetInteger("AnimationID", 3);
                agent.isStopped = false;
                agent.SetDestination(ball.transform.position);
                // Check distance to ball to pick it up
                if (Vector3.Distance(transform.position, ball.transform.position) < stopDistanceToBall)
                {
                    PickUpBall();
                }
                break;

            case PetState.ReturningBall:
                MoveToPlayer();
                if (Vector3.Distance(transform.position, userTransform.position) < stopDistanceToPlayer)
                {
                    DropBall();
                }
                break;
            case PetState.Wiggle:
                animator.SetInteger("AnimationID", 1);
                break;
            case PetState.Bark:
                animator.SetInteger("AnimationID", 6);
                break;
            case PetState.Stand:
                animator.SetInteger("AnimationID", 0);
                break;
            case PetState.Sit:
                animator.SetInteger("AnimationID", 7);
                break;
            case PetState.WalkToPlayer:
                MoveToPlayer();
                if (Vector3.Distance(transform.position, userTransform.position) < stopDistanceToPlayer)
                {
                    currentState = PetState.Idle;
                }
                break;
            case PetState.FollowPlayer:
                MoveToPlayer();
                if (Vector3.Distance(transform.position, userTransform.position) < stopDistanceToPlayer)
                {
                    agent.isStopped = true;
                    animator.SetInteger("AnimationID", 0);
                }
                break;
            case PetState.Fetch:
                //StartFetch();
                break;

        }
    }

    public void SitCommand()
    {
        StopSound();
        currentState = PetState.Sit;
    }

    public void WiggleCommand()
    {
        StopSound();
        currentState = PetState.Wiggle;
    }
    
    public void BarkCommand()
    {
        StopSound();
        currentState = PetState.Bark;
        PlaySound(barkClip);
    }
    
    public void StandCommand()
    {
        StopSound();
        currentState = PetState.Stand;
    }
    
    public void WalkToPlayerCommand()
    {
        StopSound();
        currentState = PetState.WalkToPlayer;
    }
    
    public void FollowPlayerCommand()
    {
        StopSound();
        currentState = PetState.FollowPlayer;
    }
    
    public void StartFetch(GameObject thrownBall)
    {
        StopSound();
        ball = thrownBall;
        currentState = PetState.GoingToBall;
    }

    private void PlaySound(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    private void StopSound()
    {
        audioSource.clip = null;
        audioSource.Stop();
    }

    private void MoveToPlayer()
    {
        animator.SetInteger("AnimationID", 3);
        agent.isStopped = false;
        agent.SetDestination(userTransform.position);
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
        if (ball == null) return;
        ball.transform.SetParent(null);
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;

        ball = null;
        currentState = PetState.Idle;
    }
}