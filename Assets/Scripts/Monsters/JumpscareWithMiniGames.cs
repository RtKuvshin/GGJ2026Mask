using System.Collections;
using UnityEngine;

public class JumpscareWithMiniGames : MonoBehaviour
{
    [Header("References")]
    public Jumpscare jumpscare;
    public TimingMinigame timingMinigame;
    public ScarenessLevel scareness;

    [Header("Settings")]
    public int roundsToPlay = 5;
    public float successBPMIncrease = 10f;
    public float failBPMIncrease = 0f;

    private float saveBPM = 0f;
    private int currentRound = 0;
    private int successCount = 0;

    void Awake()
    {
        
    }

    public void TriggerJumpscareSequence()
    {
        StartCoroutine(MiniGameSequence());
    }


    private IEnumerator MiniGameSequence()
    {
        if (scareness == null || timingMinigame == null) yield break;

        saveBPM = scareness.GetCurrentBPM();
        currentRound = 0;
        successCount = 0;

        while (currentRound < roundsToPlay)
        {
            scareness.OverrideBPM(200f);

            timingMinigame.StartMiniGame();

            yield return new WaitUntil(() => !timingMinigame.gameObject.activeSelf);

            scareness.OverrideBPM(scareness.GetCurrentBPM());

            if (timingMinigame.LastRoundSuccess)
                successCount++;

            currentRound++;
            yield return new WaitForSeconds(0.2f);
        }

        ApplyHeartbeatAdjustment();
    }
    

    private void ApplyHeartbeatAdjustment()
    {
        if (scareness == null) return;

        int fails = roundsToPlay - successCount;
        float newBPM = saveBPM + (successCount * successBPMIncrease) + (fails * failBPMIncrease);
        newBPM = Mathf.Clamp(newBPM, scareness.minBPM, scareness.maxBPM);

        scareness.OverrideBPM(newBPM);
        Debug.Log($"Mini-games finished! Success: {successCount}/{roundsToPlay}, BPM now: {newBPM}");
    }
}

