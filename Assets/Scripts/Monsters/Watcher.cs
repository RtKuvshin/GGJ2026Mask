using UnityEngine;

public class Watcher : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Settings")]
    public float appearRange = 50f;    // Distance where he appears
    public float hideRange = 10f;      // Distance where he hides
    public Vector3 slideOffset;        // Offset to slide out when appearing
    public float slideSpeed = 2f;      // Sliding speed
    public float arriveThreshold = 0.01f; // How close to target to consider "arrived"

    private Vector3 originalPosition;
    private Vector3 targetPosition;

    void Start()
    {
        originalPosition = transform.position;
        targetPosition = originalPosition;
        gameObject.SetActive(true); // Start hidden
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, originalPosition);

        if (distance <= hideRange)
        {
            // Player too close → hide
            targetPosition = originalPosition;
        }
        else if (distance <= appearRange)
        {
            // Within appear range → slide out
            targetPosition = originalPosition + slideOffset;
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
        }
        else
        {
            // Player far → stay hidden
            targetPosition = originalPosition;
        }

        if (gameObject.activeSelf)
        {
            // Smooth slide
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * slideSpeed);

            // Hide if arrived back at original position
            if (Vector3.Distance(transform.position, originalPosition) < arriveThreshold && distance < hideRange)
            {
                gameObject.SetActive(false);
            }
        }
    }
}