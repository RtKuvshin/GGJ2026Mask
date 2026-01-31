using UnityEngine;

[ExecuteAlways]
public class HouseGenerator : MonoBehaviour
{
    [Header("House Prefabs")]
    public GameObject[] housePrefabs;

    [Header("1st Row Settings")]
    public int firstRowCount = 5;
    public Vector3 firstRowStart = Vector3.zero;
    public float spacingX = 30f;

    [Header("2nd Row Settings")]
    public int secondRowCount = 5;
    public Vector3 secondRowStart = new Vector3(10f, 0f, -10f);
    public float secondRowRotationY = 180f;

    [ContextMenu("Generate Houses")]
    public void GenerateHouses()
    {
        ClearGenerated();

        System.Random rng = new System.Random(12345);

        // FIRST ROW (skip main house)
        for (int i = 1; i < firstRowCount; i++)
        {
            Vector3 pos = firstRowStart + new Vector3(spacingX * i, 0f, 0f);
            SpawnHouse(rng, pos, Quaternion.identity);
        }

        // SECOND ROW
        for (int i = 0; i < secondRowCount; i++)
        {
            Vector3 pos = secondRowStart + new Vector3(spacingX * i, 0f, 0f);
            SpawnHouse(rng, pos, Quaternion.Euler(0f, secondRowRotationY, 0f));
        }
    }

    void SpawnHouse(System.Random rng, Vector3 pos, Quaternion rot)
    {
        if (housePrefabs.Length == 0) return;

        GameObject prefab = housePrefabs[rng.Next(housePrefabs.Length)];
        GameObject go = Instantiate(prefab, pos, rot, transform);
        go.name = prefab.name;
    }

    void ClearGenerated()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (!Application.isPlaying)
                DestroyImmediate(transform.GetChild(i).gameObject);
            else
                Destroy(transform.GetChild(i).gameObject);
        }
    }
}