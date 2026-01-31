using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class ScarenessLevel : MonoBehaviour
{
    public enum DisplayMode { None, Slider, Heartbeat }

    [Header("General Settings")]
    public DisplayMode displayMode = DisplayMode.Heartbeat;
    public float timeToMaxMinutes = 30f; // full fear in 30 min

    [Header("Heartbeat Settings")]
    public float minBPM = 60f;
    public float maxBPM = 220f;
    
    [Header("Audio Settings")]
    public float baseVolume = 0.2f;  // minimum volume
    public float maxVolume = 1f; 

    [Header("UI References")]
    public Slider heartbeatSlider;
    public Image heartbeatImage;
    public TMP_Text heartbeatText;

    [Header("Pulse Visual")]
    public float pulseScale = 1.2f;
    public float returnSpeed = 10f;

    [Header("Line Graph")]
    public UIBPMGraph bpmGraph; // assign in inspector
    public float spikeHeightMultiplier = 1f; // optional: scale spike with fear
    
    public AudioSource audioSource;
    public AudioClip heartbeatClip1; // first beat (lub)
    public AudioClip heartbeatClip2; // second beat (dub)


    // Internal
    private float elapsedTime;
    private float normalizedValue;
    private float currentBPM;
    private Vector3 baseHeartScale;
    private float pulseTimer;

    void Awake()
    {
        if (heartbeatImage) baseHeartScale = heartbeatImage.transform.localScale;

        // init bpmGraph if missing
        if (bpmGraph == null)
        {
            bpmGraph = GetComponentInChildren<UIBPMGraph>();
            if (bpmGraph == null)
            {
                Debug.LogWarning("No UIBPMGraph found! Create one in a UI holder.");
            }
        }
    }

    void Update()
    {
        UpdateScareness();
        UpdateUI();
        UpdateHeartbeatPulse();
        UpdateHeartbeatText();
        UpdateGraph();
    }

    void UpdateScareness()
    {
        elapsedTime += Time.deltaTime;
        float maxTime = Mathf.Max(0.01f, timeToMaxMinutes) * 60f;
        normalizedValue = Mathf.Clamp01(elapsedTime / maxTime);
        currentBPM = Mathf.Lerp(minBPM, maxBPM, normalizedValue);
    }

    void UpdateUI()
    {
        switch (displayMode)
        {
            case DisplayMode.None:
                if (heartbeatSlider) heartbeatSlider.gameObject.SetActive(false);
                if (heartbeatImage) heartbeatImage.gameObject.SetActive(false);
                if (heartbeatText) heartbeatText.gameObject.SetActive(false);
                if (bpmGraph) bpmGraph.gameObject.SetActive(false);
                break;

            case DisplayMode.Slider:
                if (heartbeatSlider) heartbeatSlider.gameObject.SetActive(true);
                if (heartbeatSlider) heartbeatSlider.value = normalizedValue;
                if (heartbeatImage) heartbeatImage.gameObject.SetActive(false);
                if (heartbeatText) heartbeatText.gameObject.SetActive(false);
                if (bpmGraph) bpmGraph.gameObject.SetActive(false);
                break;

            case DisplayMode.Heartbeat:
                if (heartbeatImage) heartbeatImage.gameObject.SetActive(true);
                if (heartbeatSlider) heartbeatSlider.gameObject.SetActive(false);
                if (heartbeatText) heartbeatText.gameObject.SetActive(true);
                if (bpmGraph) bpmGraph.gameObject.SetActive(true);
                break;
        }
    }

    private float beatTimer = 0f;
    private int beatState = 0; // 0 = first beat, 1 = second beat, 2 = rest

    void UpdateHeartbeatPulse()
    {
        if (heartbeatImage == null || audioSource == null) return;

        float beatInterval = 60f / Mathf.Max(1f, currentBPM); // full cycle in seconds
        float shortDelay = 0.25f; // delay between lub and dub

        beatTimer += Time.deltaTime;

        switch (beatState)
        {
            case 0: // first beat
                PulseHeart(0); 
                beatState = 1;
                beatTimer = 0f;
                break;

            case 1: // second beat
                if (beatTimer >= shortDelay)
                {
                    PulseHeart(1); 
                    beatState = 2;
                    beatTimer = 0f;
                }
                break;

            case 2: // rest until next cycle
                if (beatTimer >= beatInterval - shortDelay)
                {
                    beatState = 0;
                    beatTimer = 0f;
                }
                break;
        }

        // smooth return scale (only if image exists)
        if (heartbeatImage != null)
        {
            heartbeatImage.transform.localScale = Vector3.Lerp(
                heartbeatImage.transform.localScale,
                baseHeartScale,
                Time.deltaTime * returnSpeed
            );
        }
    }

    void PulseHeart(int beatNumber)
    {
        // scale pulse only if image exists
        if (heartbeatImage != null)
            heartbeatImage.transform.localScale = baseHeartScale * pulseScale;

        float volume = Mathf.Lerp(baseVolume, maxVolume, normalizedValue); // louder as fear rises

        if (beatNumber == 0 && heartbeatClip1 != null)
            audioSource.PlayOneShot(heartbeatClip1, volume);
        else if (beatNumber == 1 && heartbeatClip2 != null)
            audioSource.PlayOneShot(heartbeatClip2, volume);
    }


    void UpdateHeartbeatText()
    {
        if (heartbeatText)
            heartbeatText.text = Mathf.RoundToInt(currentBPM).ToString();
    }

    void UpdateGraph()
    {
        if (bpmGraph == null) return;

        // update BPM
        bpmGraph.bpm = currentBPM;

        // optional: scale spike by fear level
        bpmGraph.pulseHeight = 60f * (1f + normalizedValue * spikeHeightMultiplier);
    }

    // Get current BPM (for audio or effects)
    public float GetCurrentBPM() => currentBPM;

    // Get normalized fear (0..1)
    public float GetFearLevel() => normalizedValue;
}
