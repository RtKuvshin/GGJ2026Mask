using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class ScarenessLevel : MonoBehaviour
{
    public enum DisplayMode { None, Slider, Heartbeat }

    [Header("General Settings")]
    public DisplayMode displayMode = DisplayMode.Heartbeat;
    public float timeToMaxMinutes = 30f;

    [Header("Atmosphere Settings")]
    [Range(0f, 1f)] public float maxDarknessAlpha = 1f;
    public bool dimLights = true;
    public bool useFog = true;

    [Header("Heartbeat Settings")]
    public float minBPM = 60f;
    public float maxBPM = 220f;

    [Header("Audio Settings")]
    public float baseVolume = 0.2f;
    public float maxVolume = 1f;

    [Header("UI References")]
    public Slider heartbeatSlider;
    public Image heartbeatImage;
    public TMP_Text heartbeatText;

    [Header("Pulse Visual")]
    public float pulseScale = 1.2f;
    public float returnSpeed = 10f;

    [Header("Line Graph")]
    public UIBPMGraph bpmGraph;
    public float spikeHeightMultiplier = 1f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip heartbeatClip1;
    public AudioClip heartbeatClip2;

    // Internal
    private float elapsedTime;
    private float normalizedValue;
    private float currentBPM;
    private Vector3 baseHeartScale;

    private float beatTimer;
    private int beatState;

    // Atmosphere runtime
    private Image runtimeDarknessMask;
    private Light directionalSun;

    void Awake()
    {
        if (heartbeatImage)
            baseHeartScale = heartbeatImage.transform.localScale;

        if (bpmGraph == null)
            bpmGraph = GetComponentInChildren<UIBPMGraph>();

        if (dimLights)
        {
            Light[] lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
            foreach (var l in lights)
            {
                if (l.type == LightType.Directional)
                {
                    directionalSun = l;
                    break;
                }
            }
        }

        SetupScriptedDarkness();
    }

    void Update()
    {
        UpdateScareness();
        UpdateAtmosphere();
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
                SetUI(false, false);
                break;

            case DisplayMode.Slider:
                SetUI(true, false);
                if (heartbeatSlider)
                    heartbeatSlider.value = normalizedValue;
                break;

            case DisplayMode.Heartbeat:
                SetUI(false, true);
                break;
        }
    }

    void SetUI(bool slider, bool heart)
    {
        if (heartbeatSlider) heartbeatSlider.gameObject.SetActive(slider);
        if (heartbeatImage) heartbeatImage.gameObject.SetActive(heart);
        if (heartbeatText) heartbeatText.gameObject.SetActive(heart);
        if (bpmGraph) bpmGraph.gameObject.SetActive(heart);
    }

    void UpdateHeartbeatPulse()
    {
        if (audioSource == null || heartbeatImage == null)
            return;

        float beatInterval = 60f / Mathf.Max(1f, currentBPM);
        float shortDelay = 0.25f;

        beatTimer += Time.deltaTime;

        switch (beatState)
        {
            case 0:
                PulseHeart(0);
                beatState = 1;
                beatTimer = 0f;
                break;

            case 1:
                if (beatTimer >= shortDelay)
                {
                    PulseHeart(1);
                    beatState = 2;
                    beatTimer = 0f;
                }
                break;

            case 2:
                if (beatTimer >= beatInterval - shortDelay)
                {
                    beatState = 0;
                    beatTimer = 0f;
                }
                break;
        }

        heartbeatImage.transform.localScale = Vector3.Lerp(
            heartbeatImage.transform.localScale,
            baseHeartScale,
            Time.deltaTime * returnSpeed
        );
    }

    void PulseHeart(int beat)
    {
        heartbeatImage.transform.localScale = baseHeartScale * pulseScale;

        float volume = Mathf.Lerp(baseVolume, maxVolume, normalizedValue);

        if (beat == 0 && heartbeatClip1)
            audioSource.PlayOneShot(heartbeatClip1, volume);
        else if (beat == 1 && heartbeatClip2)
            audioSource.PlayOneShot(heartbeatClip2, volume);
    }

    void UpdateHeartbeatText()
    {
        if (heartbeatText)
            heartbeatText.text = Mathf.RoundToInt(currentBPM).ToString();
    }

    void UpdateGraph()
    {
        if (!bpmGraph) return;

        bpmGraph.bpm = currentBPM;
        bpmGraph.pulseHeight = 60f * (1f + normalizedValue * spikeHeightMultiplier);
    }

    void UpdateAtmosphere()
    {
        if (runtimeDarknessMask)
        {
            Color c = runtimeDarknessMask.color;
            c.a = normalizedValue * maxDarknessAlpha;
            runtimeDarknessMask.color = c;
        }

        if (directionalSun)
            directionalSun.intensity = Mathf.Lerp(1f, 0.05f, normalizedValue);

        if (useFog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = Mathf.Lerp(0.001f, 0.05f, normalizedValue);
            RenderSettings.fogColor = Color.black;
        }
    }

    void SetupScriptedDarkness()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (!canvas)
        {
            var c = new GameObject("DarknessCanvas");
            canvas = c.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
            c.AddComponent<CanvasScaler>();
        }

        var maskObj = new GameObject("DynamicDarknessOverlay");
        maskObj.transform.SetParent(canvas.transform, false);

        runtimeDarknessMask = maskObj.AddComponent<Image>();
        runtimeDarknessMask.sprite = CreateVignetteSprite();
        runtimeDarknessMask.color = new Color(0, 0, 0, 0);
        runtimeDarknessMask.raycastTarget = false;

        var rt = runtimeDarknessMask.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
    }

    Sprite CreateVignetteSprite()
    {
        int size = 512;
        Texture2D tex = new Texture2D(size, size);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float maxDist = size / 2.2f;

        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float dist = Vector2.Distance(new Vector2(x, y), center);
            float a = Mathf.Clamp01((dist / maxDist) - 0.3f);
            tex.SetPixel(x, y, new Color(0, 0, 0, a * a));
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), Vector2.one * 0.5f);
    }

    public float GetCurrentBPM() => currentBPM;
    public float GetFearLevel() => normalizedValue;
}
