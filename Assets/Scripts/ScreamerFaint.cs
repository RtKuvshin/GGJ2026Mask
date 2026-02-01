using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using ElmanGameDevTools.PlayerSystem;

public class ScreamerFaint : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;
    public InnerThoughtText thoughts;
    public Image fadeImage;
    public Image screamerImage;
    public Sprite screamerSprite;
    public GameObject screamerPanel;
    public GameObject toBeContinuedPanel;
    public AudioSource screamerSource;
    public TeleportMonologueTrigger n1;
    public TeleportMonologueTrigger n2;
    

    [Header("Teleport")]
    public Transform teleportPoint;

    [Header("Message")]
    [TextArea(2, 4)]
    public string faintMessage = "…Where am I?";
    public float messageLifeTime = 4f;

    [Header("Fade")]
    public float fadeDuration = 0.6f;
    [Header("End")]
    public float quitDelay = 5f;


    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (n1.triggered && n2.triggered)
        {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;
        StartCoroutine(FaintSequence());
            
        }
    }

    IEnumerator FaintSequence()
    {
        // 🔒 Lock input
        playerController.inputLocked = true;

        // 👁 Screamer flash
        screamerPanel.gameObject.SetActive(true);
        screamerImage.sprite = screamerSprite;
        screamerSource?.Play();
        yield return new WaitForSeconds(0.15f);
        screamerPanel.gameObject.SetActive(false);

        // ⬛ Fade to black
        fadeImage.gameObject.SetActive(true);
        yield return Fade(0f, 1f);

        // 📍 Teleport player
        var cc = playerController.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;
        playerController.transform.SetPositionAndRotation(
            teleportPoint.position,
            teleportPoint.rotation
        );
        if (cc) cc.enabled = true;

        // ⬜ Fade IN
        yield return Fade(1f, 0f);

        // 💭 Thought
        thoughts.ShowThought(
            faintMessage,
            3f,
            5f,
            0.05f,
            messageLifeTime
        );

        yield return new WaitForSeconds(messageLifeTime + 1f);

        // ⬛ Fade OUT again
        yield return Fade(0f, 1f);

        // 📜 To be continued
        toBeContinuedPanel.SetActive(true);
        yield return new WaitForSeconds(quitDelay);
        ForceQuit();
    }

    IEnumerator Fade(float from, float to)
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
    void ForceQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();

    // absolute fallback (Windows)
    Process.GetCurrentProcess().Kill();
#endif
    }

}
