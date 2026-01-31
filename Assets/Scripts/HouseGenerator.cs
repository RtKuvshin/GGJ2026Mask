using UnityEngine;

public class HouseGenerator : MonoBehaviour
{
    [Header("House Prefabs")]
    public GameObject[] housePrefabs; // assign all prefabs here

    [Header("1st Row Settings")]
    public int firstRowCount = 5; // including main house
    public Vector3 firstRowStart = Vector3.zero;
    public float spacingX = 30f;

    [Header("2nd Row Settings")]
    public int secondRowCount = 5;
    public Vector3 secondRowStart = new Vector3(10f, 0f, -10f);
    public float secondRowRotationY = 180f;

    [Header("Generation Control")]
    public bool generateOnStart = true;

    private bool generated = false;

    void Start()
    {
        if (generateOnStart && !generated)
        {
            GenerateHouses();
            generated = true;
        }
    }

    void GenerateHouses()
    {
        // Make deterministic random
        System.Random rng = new System.Random(12345); // same seed every time

        // ---------- FIRST ROW ----------
        // First house already exists at firstRowStart (do not touch)
        for (int i = 1; i < firstRowCount; i++) // skip main house
        {
            Vector3 pos = firstRowStart + new Vector3(spacingX * i, 0f, 0f);
            GameObject prefab = housePrefabs[rng.Next(housePrefabs.Length)];
            Instantiate(prefab, pos, Quaternion.identity, transform);
        }

        // ---------- SECOND ROW ----------
        for (int i = 0; i < secondRowCount; i++)
        {
            Vector3 pos = secondRowStart + new Vector3(spacingX * i, 0f, 0f);
            GameObject prefab = housePrefabs[rng.Next(housePrefabs.Length)];
            Instantiate(prefab, pos, Quaternion.Euler(0f, secondRowRotationY, 0f), transform);
        }
    }
}