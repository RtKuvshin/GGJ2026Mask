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

    void Awake()
    {
        textMesh.text = "";
    }

    /// <summary>
    /// Shows inner thoughts with typing + wobble effect
    /// </summary>
    public void ShowThought(
        string text,
        float letterMoveHeight = 4f,
        float letterMoveSpeed = 4f,
        float typingSpeed = 0.04f,
        float lifeTime = 3f
    )
    {
        if (typingRoutine != null) StopCoroutine(typingRoutine);
        if (wobbleRoutine != null) StopCoroutine(wobbleRoutine);

        typingRoutine = StartCoroutine(TypeText(text, typingSpeed, letterMoveHeight, letterMoveSpeed, lifeTime));
    }

    IEnumerator TypeText(string fullText, float typingSpeed, float moveHeight, float moveSpeed, float lifeTime)
    {
        textBg.SetActive(true);
        textMesh.text = "";
        textMesh.ForceMeshUpdate();

        for (int i = 0; i <= fullText.Length; i++)
        {
            textMesh.text = fullText.Substring(0, i);
            yield return new WaitForSeconds(typingSpeed);
        }

        wobbleRoutine = StartCoroutine(WobbleText(moveHeight, moveSpeed));

        yield return new WaitForSeconds(lifeTime);

        StopCoroutine(wobbleRoutine);
        textMesh.text = "";
        textBg.SetActive(false);
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
