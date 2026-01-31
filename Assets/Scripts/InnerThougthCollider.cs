using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ElmanGameDevTools.PlayerSystem;
using TMPro;

public class ThoughtFadeTrigger : MonoBehaviour
{
    [Header("Collider Settings")]
    public string targetColliderName = "BackCollider";

    [Header("Player")]
    public GameObject playerControllerObj;
    public Transform player;
    public CharacterController characterController;

    [Header("Fade Settings")]
    public Image fadeImage;
    public float fadeDuration = 0.5f;
    public float waitAfterFade = 2f;

    [Header("Inner Thought")]
    public string message = "I have to find my mask. Going to my neighbours won't help me...";
    [SerializeField] private InnerThoughtText thoughts;

    private PlayerController playerController;
    private bool busy = false;

    private void Awake()
    {
        playerController = playerControllerObj.GetComponent<PlayerController>();
    }

    // Detect collision using CharacterController
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (busy) return;
        if (hit.collider.name != targetColliderName) return;

        StartCoroutine(TriggerSequence());
    }

    private IEnumerator TriggerSequence()
    {
        busy = true;

        // LOCK MOVEMENT
        playerController.inputLocked = true;

        // SHOW INNER THOUGHT
        thoughts.ShowThought(message, 3f, 5f, 0.05f);

        // WAIT until thought disappears
        yield return new WaitForSeconds(4f);

        // FADE TO BLACK
        yield return StartCoroutine(Fade(0f, 1f));

        // WAIT AFTER FADE (e.g., 2 sec)
        yield return new WaitForSeconds(waitAfterFade);

        // FADE BACK TO NORMAL
        yield return StartCoroutine(Fade(1f, 0f));
        fadeImage.gameObject.SetActive(false);

        // UNLOCK MOVEMENT
        playerController.inputLocked = false;

        busy = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        fadeImage.gameObject.SetActive(true);
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
