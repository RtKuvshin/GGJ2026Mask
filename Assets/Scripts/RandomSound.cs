using UnityEngine;

public class RandomSound : MonoBehaviour
{
    [Header("References")]
    public ScarenessLevel scareness; // reference to your ScarenessLevel
    public AudioSource audioSource;
    public AudioClip[] clips;

    [Header("Chance Settings")]
    [Range(0f, 1f)] public float minChancePerSecond = 0.01f; // very rare at low BPM
    [Range(0f, 1f)] public float maxChancePerSecond = 0.2f;  // more likely at high BPM

    private void Update()
    {
        if (!scareness || !audioSource || clips.Length == 0) return;

        // normalized BPM (0 = minBPM, 1 = maxBPM)
        float bpmNorm = (scareness.GetCurrentBPM() - scareness.minBPM) / (scareness.maxBPM - scareness.minBPM);
        bpmNorm = Mathf.Clamp01(bpmNorm);

        // chance this frame (per second * deltaTime)
        float chance = Mathf.Lerp(minChancePerSecond, maxChancePerSecond, bpmNorm) * Time.deltaTime;

        if (Random.value < chance)
        {
            // pick a random clip
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            audioSource.PlayOneShot(clip);
        }
    }
}