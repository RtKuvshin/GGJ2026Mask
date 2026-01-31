using UnityEngine;

[ExecuteAlways]
public class CircularHouseGenerator : MonoBehaviour
{
    [Header("House Prefabs")]
    public GameObject[] housePrefabs;

    [Header("C-Shape Settings")]
    public int houseCount = 8;
    public float radius = 20f;
    public Vector3 centerOffset = Vector3.zero;
    public float startAngle = 0f;
    public float endAngle = 180f;

    [Header("Generation Control")]
    [Tooltip("Use the context menu button in Edit Mode.")]
    public bool generateOnStart = false;

    // Context menu generator
    [ContextMenu("Generate C-Houses")]
    public void GenerateHouses()
    {
        if (housePrefabs == null || housePrefabs.Length == 0)
        {
            Debug.LogError("CircularHouseGenerator: No prefabs assigned!");
            return;
        }

        ClearGenerated();

        System.Random rng = new System.Random(12345);
        Vector3 center = transform.position + centerOffset;

        for (int i = 0; i < houseCount; i++)
        {
            float t = (float)i / (houseCount - 1);
            float angle = Mathf.Lerp(startAngle + 180f, endAngle + 180f, t) * Mathf.Deg2Rad;

            Vector3 pos = center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
            Quaternion rot = Quaternion.LookRotation(center - pos, Vector3.up) * Quaternion.Euler(0f, 180f, 0f);

            GameObject prefab = housePrefabs[rng.Next(housePrefabs.Length)];
            GameObject go = Instantiate(prefab, pos, rot, transform);
            go.name = prefab.name;
            go.AddComponent<GeneratedHouse>(); // mark it
        }
    }

    [ContextMenu("Clear Generated Houses")]
    public void ClearGenerated()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.GetComponent<GeneratedHouse>() != null)
            {
                if (!Application.isPlaying)
                    DestroyImmediate(child.gameObject);
                else
                    Destroy(child.gameObject);
            }
        }
    }
}

// marker component
public class GeneratedHouse : MonoBehaviour {}
