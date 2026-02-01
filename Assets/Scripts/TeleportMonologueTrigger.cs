using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events; // <-- Add this
using ElmanGameDevTools.PlayerSystem;

public class TeleportMonologueTrigger : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;
    public CharacterController characterController;
    public InnerThoughtText thoughts;
    public Image fadeImage;

    [Header("Teleport Settings")]
    public Transform teleportTarget;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;
    public float blackScreenDelay = 0.3f;

    [Header("Monologue")]
    [TextArea(2, 4)] public string[] messages;
    public float letterHeight = 3f;
    public float letterSpeed = 5f;
    public float typingSpeed = 0.05f;
    public float messageLifeTime = 3.5f;
    public float delayBetweenMessages = 0.5f;

    [Header("Custom Event")]
    public UnityEvent onTriggerEvent; // <-- This is the new event

    public bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            // Call the custom event
            onTriggerEvent?.Invoke();

            StartCoroutine(TeleportSequence(other.transform));
        }
    }

    private IEnumerator TeleportSequence(Transform player)
    {
        // 🔒 Lock player input
        playerController.inputLocked = true;

        // Fade out
        fadeImage.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(0f, 1f));

        // Optional delay for horror effect
        yield return new WaitForSeconds(blackScreenDelay);

        // Teleport player
        if (characterController) characterController.enabled = false;
        player.position = teleportTarget.position;
        player.rotation = teleportTarget.rotation;
        if (characterController) characterController.enabled = true;

        // Show monologue
        thoughts.ShowThoughts(
            messages,
            letterHeight,
            letterSpeed,
            typingSpeed,
            messageLifeTime
        );

        yield return new WaitForSeconds(messageLifeTime + delayBetweenMessages);

        // Fade in
        yield return StartCoroutine(Fade(1f, 0f));
        fadeImage.gameObject.SetActive(false);

        // 🔓 Unlock input
        playerController.inputLocked = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = to;
        fadeImage.color = c;
    }
}
