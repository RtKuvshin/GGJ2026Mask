using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShadowStalker : MonoBehaviour
{
    [Header("References")]
    public Transform player;          
    public Camera playerCamera;       
    public AudioClip screamerClip;    

    [Header("Settings")]
    public float followDelay = 0.5f;   
    public float distanceGap = 1f;     
    public float detectionAngle = 60f; 

    public float initialScreamerDelay = 10f; // 1 minute before first screamer
    public float repeatScreamerDelay = 180f; // repeat every 3 minutes if not seen

    private Vector3 lastPlayerPos;
    private AudioSource audioSource;
    private bool isActive = false;
    private bool screamerPlayed = false;

    void Start()
    {
        if (!player) Debug.LogError("ShadowStalker: Player not assigned!");
        if (!playerCamera) Debug.LogError("ShadowStalker: Camera not assigned!");

        audioSource = GetComponent<AudioSource>();
        lastPlayerPos = transform.position;
        gameObject.SetActive(false); // start disabled until activated
    }

    void Update()
    {
        if (!isActive) return;

        // Move behind player with a small lag
        Vector3 targetPos = Vector3.Lerp(transform.position, lastPlayerPos, Time.deltaTime / followDelay);

        // Keep a minimum distance gap
        if (Vector3.Distance(targetPos, player.position) < distanceGap)
        {
            Vector3 dir = (targetPos - player.position).normalized;
            targetPos = player.position + dir * distanceGap;
        }

        transform.position = targetPos;
        transform.LookAt(player.position);

        // Update last player position
        lastPlayerPos = player.position;
    }

    /// <summary>
    /// Call this to activate the Shadow Stalker
    /// </summary>
    public void ActivateSS()
    {
        if (isActive) return;
        isActive = true;

        // Teleport right behind the player
        Vector3 behindPos = player.position - player.forward * distanceGap;
        transform.position = behindPos;

        // Face the player
        transform.LookAt(player.position);

        gameObject.SetActive(true);

        // Start the screamer coroutine
        StartCoroutine(ScreamerRoutine());
    }

    private IEnumerator ScreamerRoutine()
    {
        yield return new WaitForSeconds(initialScreamerDelay);

        while (true)
        {
            if (!PlayerSeesMonster())
            {
                TriggerScreamer();
            }

            // Wait 3 minutes before next check
            yield return new WaitForSeconds(repeatScreamerDelay);
        }
    }

    private bool PlayerSeesMonster()
    {
        Vector3 viewportPos = playerCamera.WorldToViewportPoint(transform.position);
        if (viewportPos.z < 0) return false; // behind camera
        if (viewportPos.x < 0 || viewportPos.x > 1) return false;
        if (viewportPos.y < 0 || viewportPos.y > 1) return false;

        // Check angle too
        Vector3 toMonster = (transform.position - playerCamera.transform.position).normalized;
        float angle = Vector3.Angle(playerCamera.transform.forward, toMonster);
        return angle < detectionAngle / 2f;
    }

    private void TriggerScreamer()
    {
        if (screamerClip && audioSource)
        {
            audioSource.PlayOneShot(screamerClip);
            screamerPlayed = true;

            // Optional: flash screen, shake camera, etc.
        }
    }
}
