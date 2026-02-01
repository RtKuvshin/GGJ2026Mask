using UnityEngine;
using UnityEngine.UI;

public class UltimateController : MonoBehaviour
{
    public static UltimateController Instance;

    [Header("UI References")]
    public GameObject screamerPanel;
    public Image screamerImage;

    [Header("Player Reference")]
    public Transform player;
    
    public TimingMinigame timingMinigame;
    public ScarenessLevel scareness;
    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
}

