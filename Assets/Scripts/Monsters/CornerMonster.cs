using UnityEngine;

public class CornerWatcher : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject watcherVisual;

    [Header("Corner Settings")]
    public Transform[] cornerSpots;
    public float appearDistance = 40f;
    public float hideDistance = 8f;

    [Header("Stare Settings")]
    public float stareAngle = 12f;
    public float fearPerSecond = 1.5f;

    [Header("Jumpscare")]
    public float jumpscareRange = 2f;
    public float jumpscareCooldown = 60f;

    int currentCorner = -1;
    bool isVisible;
    bool canJumpscare;
    float lastJumpscareTime;

    void Start()
    {
        watcherVisual.SetActive(false);
    }

    void Update()
    {
        if (!isVisible)
        {
            TrySpawn();
            return;
        }

        FacePlayer();
        HandleStare();
        HandleHide();
        HandleJumpscare();
    }

    void TrySpawn()
    {
        for (int i = 0; i < cornerSpots.Length; i++)
        {
            float dist = Vector3.Distance(player.position, cornerSpots[i].position);

            if (dist < appearDistance && !PlayerLookingAt(cornerSpots[i]))
            {
                currentCorner = i;
                transform.position = cornerSpots[i].position;
                watcherVisual.SetActive(true);
                isVisible = true;
                canJumpscare = true;
                break;
            }
        }
    }

    void HandleHide()
    {
        float dist = Vector3.Distance(player.position, transform.position);

        if (dist < hideDistance)
        {
            watcherVisual.SetActive(false);
            isVisible = false;
        }
    }

    void HandleStare()
    {
        if (PlayerLookingAt(transform))
        {
            // Hook this into your fear system
            // FearManager.AddFear(fearPerSecond * Time.deltaTime);
        }
    }

    void HandleJumpscare()
    {
        if (!canJumpscare) return;
        if (Time.time - lastJumpscareTime < jumpscareCooldown) return;

        float dist = Vector3.Distance(player.position, transform.position);

        if (dist < jumpscareRange)
        {
            // VERY short visual flash
            watcherVisual.SetActive(true);
            Invoke(nameof(HideAfterScare), 0.15f);

            lastJumpscareTime = Time.time;
            canJumpscare = false;
        }
    }

    void HideAfterScare()
    {
        watcherVisual.SetActive(false);
    }

    void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    bool PlayerLookingAt(Transform target)
    {
        Vector3 dirToTarget = (target.position - player.position).normalized;
        float angle = Vector3.Angle(player.forward, dirToTarget);
        return angle < stareAngle;
    }
}
