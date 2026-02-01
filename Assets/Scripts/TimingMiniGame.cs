using UnityEngine;
using UnityEngine.UI;

public class TimingMinigame : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform barRect;
    public RectTransform targetRect;
    public RectTransform pointerRect;

    [Header("Settings")]
    public float speed = 500f;
    public float targetWidth = 50f;

    private float _currentPos;
    private int _direction = 1;
    private float _barWidth;
    public AudioSource audioSource;
    public AudioClip successClip;


    public bool IsActive { get; private set; } = false;
    public bool LastRoundSuccess { get; private set; }

    public void StartMiniGame()
    {
        _barWidth = barRect.rect.width;
        RandomizeTarget();

        _currentPos = 0;
        _direction = 1;
        IsActive = true;
        LastRoundSuccess = false;

        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!IsActive) return;

        MovePointer();

        if (Input.GetMouseButtonDown(0))
        {
            CheckSuccess();
        }
    }

    void MovePointer()
    {
        _currentPos += speed * _direction * Time.deltaTime;

        // Bounce inside the bar
        if (_currentPos > _barWidth)
        {
            _currentPos = _barWidth;
            _direction = -1;
        }
        else if (_currentPos < 0)
        {
            _currentPos = 0;
            _direction = 1;
        }

        pointerRect.anchoredPosition = new Vector2(_currentPos, 0);
    }

    void RandomizeTarget()
    {
        float maxX = Mathf.Max(0, barRect.rect.width - targetWidth);
        float randomX = Random.Range(0, maxX);
        targetRect.anchoredPosition = new Vector2(randomX, 0);
        targetRect.sizeDelta = new Vector2(targetWidth, targetRect.sizeDelta.y);
    }

    void CheckSuccess()
    {
        IsActive = false;

        // Get the world corners of the target
        Vector3[] targetCorners = new Vector3[4];
        targetRect.GetWorldCorners(targetCorners);
        float targetLeft = targetCorners[0].x;
        float targetRight = targetCorners[2].x;

        // Get the pointer world X
        float pointerX = pointerRect.position.x;

        LastRoundSuccess = pointerX >= targetLeft && pointerX <= targetRight;

        Debug.Log(LastRoundSuccess ? "Success!" : "Failed!");

        if (LastRoundSuccess && successClip != null)
        {
            audioSource.PlayOneShot(successClip);
        }

        gameObject.SetActive(false);
    }

}
