using System.Collections;
using UnityEngine;
using TMPro;

public class InnerThoughtText : MonoBehaviour
{
    [SerializeField] private GameObject textBg;
    [SerializeField] TMP_Text textMesh;
    Mesh mesh;
    Vector3[] vertices;

    Coroutine typingRoutine;
    Coroutine wobbleRoutine;

    string[] currentMessages;
    int currentIndex;
    float letterMoveHeight;
    float letterMoveSpeed;
    float typingSpeed;
    float messageLifeTime;

    void Awake()
    {
        textMesh.text = "";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && typingRoutine != null)
        {
            SkipCurrentThought();
        }
    }

    /// <summary>
    /// Starts showing a series of thoughts
    /// </summary>
    public void ShowThoughts(
        string[] messages,
        float letterMoveHeight = 4f,
        float letterMoveSpeed = 4f,
        float typingSpeed = 0.04f,
        float messageLifeTime = 3f
    )
    {
        if (typingRoutine != null) StopCoroutine(typingRoutine);
        if (wobbleRoutine != null) StopCoroutine(wobbleRoutine);

        currentMessages = messages;
        currentIndex = 0;
        this.letterMoveHeight = letterMoveHeight;
        this.letterMoveSpeed = letterMoveSpeed;
        this.typingSpeed = typingSpeed;
        this.messageLifeTime = messageLifeTime;

        StartCoroutine(ShowNextThought());
    }
    public void ShowThought(
        string message,
        float letterMoveHeight = 4f,
        float letterMoveSpeed = 4f,
        float typingSpeed = 0.04f,
        float lifeTime = 3f
    )
    {
        ShowThoughts(new string[] { message }, letterMoveHeight, letterMoveSpeed, typingSpeed, lifeTime);
    }


    private IEnumerator ShowNextThought()
    {
        if (currentIndex >= currentMessages.Length)
        {
            // done with all thoughts
            textMesh.text = "";
            textBg.SetActive(false);
            yield break;
        }

        string fullText = currentMessages[currentIndex];
        currentIndex++;

        textBg.SetActive(true);
        textMesh.text = "";
        textMesh.ForceMeshUpdate();

        // typing
        typingRoutine = StartCoroutine(TypeText(fullText, typingSpeed));

        yield return typingRoutine;

        // wobble
        wobbleRoutine = StartCoroutine(WobbleText(letterMoveHeight, letterMoveSpeed));

        yield return new WaitForSeconds(messageLifeTime);

        if (wobbleRoutine != null)
        {
            StopCoroutine(wobbleRoutine);
            wobbleRoutine = null;
        }

        textMesh.text = "";

        // automatically go to next
        StartCoroutine(ShowNextThought());
    }

    private IEnumerator TypeText(string fullText, float speed)
    {
        for (int i = 0; i <= fullText.Length; i++)
        {
            textMesh.text = fullText.Substring(0, i);
            yield return new WaitForSeconds(speed);
        }
    }

    private void SkipCurrentThought()
    {
        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
            typingRoutine = null;
        }

        if (wobbleRoutine != null)
        {
            StopCoroutine(wobbleRoutine);
            wobbleRoutine = null;
        }

        // immediately show full text (optional)
        if (currentIndex > 0)
            textMesh.text = currentMessages[currentIndex - 1];

        // hide text and start next thought
        textBg.SetActive(false);

        // start next thought
        StartCoroutine(ShowNextThought());
    }

    IEnumerator WobbleText(float height, float speed)
    {
        while (true)
        {
            textMesh.ForceMeshUpdate();
            mesh = textMesh.mesh;
            vertices = mesh.vertices;

            for (int i = 0; i < textMesh.textInfo.characterCount; i++)
            {
                var charInfo = textMesh.textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int index = charInfo.vertexIndex;
                Vector3 offset = Vector3.up * Mathf.Sin(Time.time * speed + i * 0.3f) * height;

                vertices[index + 0] += offset;
                vertices[index + 1] += offset;
                vertices[index + 2] += offset;
                vertices[index + 3] += offset;
            }

            mesh.vertices = vertices;
            textMesh.canvasRenderer.SetMesh(mesh);

            yield return null;
        }
    }
}
