using UnityEngine;

public class LightDetection : MonoBehaviour
{
    [Header("Rotation (set speed to 0 to disable)")]
    public float rotationSpeed = 45f;

    [Header("Side-to-side patrol (set range to 0 to disable)")]
    public float patrolRange = 0f;
    public float patrolSpeed = 2f;

    [Header("Detection")]
    public float detectionTime = 2f;

    [HideInInspector] public bool isDetected = false;

    private float detectionTimer = 0f;
    private Vector3 startPos;
    private int patrolDir = 1;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Rotate the light cone
        if (rotationSpeed != 0f)
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        // Patrol left-right
        if (patrolRange > 0f)
        {
            transform.position += Vector3.right * patrolDir * patrolSpeed * Time.deltaTime;
            if (Mathf.Abs(transform.position.x - startPos.x) >= patrolRange)
                patrolDir *= -1;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        isDetected = true;
        detectionTimer += Time.deltaTime;

        if (detectionTimer >= detectionTime && GameManager.Instance != null)
            GameManager.Instance.Lose();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        isDetected = false;
        detectionTimer = 0f;
    }
}
