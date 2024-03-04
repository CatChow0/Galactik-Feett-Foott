using System.Collections;
using UnityEngine;

public class DroneAI : MonoBehaviour
{
    public Transform ball;
    public float moveSpeed = 2f;
    public float wanderRadius = 5f;
    public float retrieveProbability = 0.2f; // Probability of retrieving the ball (0 to 1)
    public float delayAfterTouching = 0.5f;

    private Vector3 initialPosition;
    private Vector3 targetPosition;

    private bool isReturning;

    void Start()
    {
        initialPosition = transform.position;
        StartCoroutine(WanderAround());
    }

    IEnumerator WanderAround()
    {
        while (true)
        {
            if (Random.value < retrieveProbability)
            {
                yield return StartCoroutine(RetrieveAndReplaceBall());
            }
            yield return StartCoroutine(MoveToRandomPosition());
        }
    }

    IEnumerator MoveToRandomPosition()
    {
        Vector3 startPosition = transform.position;
        targetPosition = initialPosition + Random.insideUnitSphere * wanderRadius;
        targetPosition.y = startPosition.y; // Keep the same height

        float journeyLength = Vector3.Distance(startPosition, targetPosition);
        float startTime = Time.time;

        while (transform.position != targetPosition)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
            yield return null;
        }
    }

    IEnumerator RetrieveAndReplaceBall()
    {
        // Move towards the ball
        yield return StartCoroutine(MoveToRandomPosition());

        // Retrieve the ball
        RetrieveBall();

        // Wait before placing the ball
        yield return new WaitForSeconds(delayAfterTouching);

        // Place the ball at a random position
        ReplaceBallRandomly();

        // Move back to initial position
        yield return StartCoroutine(MoveToInitialPosition());
    }

    void RetrieveBall()
    {
        targetPosition = ball.position;
        transform.position = targetPosition;

        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        if (ballRb != null)
        {
            ballRb.useGravity = false;
        }
    }

    void ReplaceBallRandomly()
    {
        Vector3 randomPos = Random.insideUnitSphere * wanderRadius;
        randomPos.y = ball.position.y; // Keep the same height
        ball.position = randomPos;
    }

    IEnumerator MoveToInitialPosition()
    {
        Vector3 startPosition = transform.position;
        float journeyLength = Vector3.Distance(startPosition, initialPosition);
        float startTime = Time.time;

        while (transform.position != initialPosition)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, initialPosition, fractionOfJourney);
            yield return null;
        }
    }
}
