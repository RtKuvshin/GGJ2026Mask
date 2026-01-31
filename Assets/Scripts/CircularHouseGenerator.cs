using UnityEngine;

public class CircularHouseGenerator : MonoBehaviour
{
    [Header("House Prefabs")]
    public GameObject[] housePrefabs;

    [Header("C-Shape Settings")]
    public int houseCount = 8;          // number of houses along the arc
    public float radius = 20f;          // radius of the arc
    public Vector3 centerOffset = Vector3.zero; // center of the "C" relative to the generator
    public float startAngle = 0f;       // start angle in degrees
    public float endAngle = 180f;       // end angle in degrees

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
        System.Random rng = new System.Random(12345);
        Vector3 center = transform.position + centerOffset;

        for (int i = 0; i < houseCount; i++)
        {
            float t = (float)i / (houseCount - 1); 
            float angle = Mathf.Lerp(startAngle, endAngle, t) * Mathf.Deg2Rad;

            Vector3 pos = center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;

            // rotate to face the center, then flip 180 so doors face outward
            Quaternion rot = Quaternion.LookRotation(center - pos, Vector3.up) * Quaternion.Euler(0f, 180f, 0f);

            GameObject prefab = housePrefabs[rng.Next(housePrefabs.Length)];
            Instantiate(prefab, pos, rot, transform);
        }
    }

}