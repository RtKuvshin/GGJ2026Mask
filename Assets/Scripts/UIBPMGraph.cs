using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBPMGraph : MaskableGraphic
{
    [Header("BPM Settings")]
    public float bpm = 72f;
    public float pulseHeight = 60f;

    [Header("Line Settings")]
    public float thickness = 3f;
    public int resolution = 150; 
    [Range(0.01f, 0.5f)]
    public float spikeWidth = 0.15f; 

    private List<float> yPositions = new List<float>();
    private float timer;

    protected override void OnEnable()
    {
        base.OnEnable();
        yPositions.Clear();
        for (int i = 0; i < resolution; i++) yPositions.Add(0f);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float beatInterval = 60f / bpm;
        if (timer >= beatInterval) timer -= beatInterval;

        float phase = timer / beatInterval;
        float currentY = 0;

        // Sharp Multi-Stage Spike (Up then Down)
        if (phase < spikeWidth)
        {
            float normalizedPhase = phase / spikeWidth; // 0 to 1 inside the spike window

            if (normalizedPhase < 0.33f) // First 33%: Sharp Up
            {
                currentY = (normalizedPhase / 0.33f) * pulseHeight;
            }
            else if (normalizedPhase < 0.66f) // Middle 33%: Sharp Down past baseline
            {
                float localPhase = (normalizedPhase - 0.33f) / 0.33f;
                currentY = Mathf.Lerp(pulseHeight, -pulseHeight * 0.4f, localPhase);
            }
            else // Last 33%: Return to baseline
            {
                float localPhase = (normalizedPhase - 0.66f) / 0.34f;
                currentY = Mathf.Lerp(-pulseHeight * 0.4f, 0, localPhase);
            }
        }

        // Add new point to the START (index 0) and remove the last
        yPositions.Insert(0, currentY);
        if (yPositions.Count > resolution)
            yPositions.RemoveAt(yPositions.Count - 1);

        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        
        // Use the RectTransform to determine drawing area
        float width = rectTransform.rect.width;
        float heightOffset = rectTransform.rect.height / 2; // Center the baseline vertically
        float spacing = width / (resolution - 1);

        // Draw from Left (index 0) to Right
        for (int i = 0; i < yPositions.Count - 1; i++)
        {
            Vector2 p1 = new Vector2(i * spacing, yPositions[i]);
            Vector2 p2 = new Vector2((i + 1) * spacing, yPositions[i + 1]);
            
            // Adjust to start from left side of container
            p1.x += rectTransform.rect.xMin;
            p2.x += rectTransform.rect.xMin;

            DrawLineSegment(p1, p2, vh);
        }
    }

    void DrawLineSegment(Vector2 start, Vector2 end, VertexHelper vh)
    {
        Vector2 dir = (end - start).normalized;
        Vector2 normal = new Vector2(-dir.y, dir.x) * (thickness / 2f);

        UIVertex v = UIVertex.simpleVert;
        v.color = color;

        v.position = start - normal; vh.AddVert(v);
        v.position = start + normal; vh.AddVert(v);
        v.position = end + normal; vh.AddVert(v);
        v.position = end - normal; vh.AddVert(v);

        int count = vh.currentVertCount;
        vh.AddTriangle(count - 4, count - 3, count - 2);
        vh.AddTriangle(count - 4, count - 2, count - 1);
    }
}