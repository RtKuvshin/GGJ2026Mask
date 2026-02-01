using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ElmanGameDevTools.PlayerSystem;
using TMPro;

public class DoorTeleportRaycast : MonoBehaviour
{
    [Header("Interaction")]
    public Camera playerCamera;
    public GameObject playerControllerObj;
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;
    public KeyCode interactKeyGamepad = KeyCode.JoystickButton2;
    public KeyCode interactKeyMouse = KeyCode.Mouse0;

    [Header("Teleport")]
    public Transform player;
    public Transform teleportTarget;
    public CharacterController characterController;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeDuration = 0.5f;

    [Header("Horror Timing")]
    public float blackScreenDelay = 0.3f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip doorSound;

    [Header("UI Text")]
    public string message = "I have to find my mask. I can't live without it...";
    public float textDisplayTime = 2.5f;
    [SerializeField] InnerThoughtText thoughts;

    private PlayerController playerController;
    private bool busy = false;
    private bool usedOnce = false;

    private void Awake()
    {
        playerController = playerControllerObj.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (busy) return;
        

        if (Input.GetKeyDown(interactKey) || Input.GetKeyDown(interactKeyGamepad) || Input.GetKeyDown(interactKeyMouse))
        {
            if (Physics.Raycast(
                playerCamera.transform.position,
                playerCamera.transform.forward,
                out RaycastHit hit,
                interactDistance))
            {
                if (hit.transform == transform)
                {
                    if (!usedOnce)
                    {
                        StartCoroutine(TeleportSequence());
                    }
                    else
                    {
                        thoughts.ShowThought(
                            message,
                            3f,
                            5f,
                            0.05f,
                            4f
                        );
                    }
                }

            }
        }
    }

    IEnumerator TeleportSequence()
    {
        busy = true;

        // LOCK INPUT
        playerController.inputLocked = true;

        // Fade to black
        yield return StartCoroutine(Fade(0f, 1f));

        // Sound
        if (doorSound)
            audioSource.PlayOneShot(doorSound);

        yield return new WaitForSeconds(blackScreenDelay);

        // Teleport
        if (characterController)
            characterController.enabled = false;

        player.position = teleportTarget.position;

        if (characterController)
            characterController.enabled = true;

        yield return new WaitForSeconds(6f);

        // Fade back
        yield return StartCoroutine(Fade(1f, 0f));
        fadeImage.gameObject.SetActive(false);

        // UNLOCK INPUT
        playerController.inputLocked = false;

        usedOnce = true;   // 🔒 FIRST USE DONE
        busy = false;

        yield return new WaitForSeconds(30f); ;

    }

   
    IEnumerator Fade(float from, float to)
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
