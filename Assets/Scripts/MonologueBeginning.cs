using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ElmanGameDevTools.PlayerSystem;

public class MonologueBeginning : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;
    public InnerThoughtText thoughts;
    public Image fadeImage; // full-screen black image

    [Header("Monologue")]
    [TextArea(2, 4)]
    public string[] messages;

    public float letterHeight = 3f;
    public float letterSpeed = 5f;
    public float typingSpeed = 0.05f;
    public float messageLifeTime = 3.5f;
    public float delayBetweenMessages = 0.5f;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;

    private void Start()
    {
        StartCoroutine(BeginningSequence());
    }

    private IEnumerator BeginningSequence()
    {
        // 🔒 LOCK INPUT
        playerController.inputLocked = true;

        // make sure fade image is visible and fully black
        fadeImage.gameObject.SetActive(true);
        SetFadeAlpha(1f);

        yield return new WaitForSeconds(0.2f); // tiny delay for black to settle

        // show all messages
        thoughts.ShowThoughts(
                messages,
                letterHeight,
                letterSpeed,
                typingSpeed,
                messageLifeTime
            );

            yield return new WaitForSeconds(messageLifeTime + delayBetweenMessages);
        

        // fade in
        yield return StartCoroutine(Fade(1f, 0f));

        fadeImage.gameObject.SetActive(false);

        // 🔓 UNLOCK INPUT
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

    private void SetFadeAlpha(float a)
    {
        Color c = fadeImage.color;
        c.a = a;
        fadeImage.color = c;
    }
}
