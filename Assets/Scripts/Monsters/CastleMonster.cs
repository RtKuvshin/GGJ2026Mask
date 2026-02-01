using UnityEngine;
using ElmanGameDevTools.PlayerSystem;

public class CastleMonster : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float fearMultiplier = 3f;

    private ScarenessLevel scareness;

    private void Start()
    {
        scareness = player.GetComponent<ScarenessLevel>();
        if (scareness == null) Debug.LogError("Player missing ScarenessLevel!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            scareness.fearIncreaseMultiplier = fearMultiplier;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            scareness.fearIncreaseMultiplier = 1f; // back to normal
    }
}