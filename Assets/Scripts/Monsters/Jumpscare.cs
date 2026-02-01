using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Jumpscare : MonoBehaviour
{
    [Header("References")]
    public Image screamerImage;      
    public Sprite screamerSprite;      
    public GameObject screamerPanel;      
    public AudioClip jumpscareSound;
    
    [Header("MiniGame")]
    public JumpscareWithMiniGames jumpscareWithMiniGames;

    private AudioSource audioSource;
    private Vector3 startPos;
    private bool hasPlayed = false; 

    void Awake()
    {
        screamerPanel.gameObject.SetActive(false);
        startPos = screamerImage.rectTransform.anchoredPosition;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void TriggerJumpscare()
    {
        if (hasPlayed) return; 
        hasPlayed = true;

        screamerPanel.gameObject.SetActive(true);
        screamerImage.sprite = screamerSprite;

        if (jumpscareSound) audioSource.PlayOneShot(jumpscareSound, 1f);

        var rect = screamerImage.rectTransform;

        // Start tiny and invisible
        rect.localScale = Vector3.zero;
        rect.anchoredPosition = startPos;

        // Animate scale to full size in a "popping" way
        Sequence jumpSequence = DOTween.Sequence();

        // Pop in 1 frame (instant)
        jumpSequence.Append(rect.DOScale(0.1f, 0f));

        // Expand quickly to full size (10-15 frames ~ 0.15-0.25s)
        jumpSequence.Append(rect.DOScale(1f, 1f).SetEase(Ease.OutBack));
        

        // Hide after short time
        jumpSequence.OnComplete(() =>
        {
            Hide();
            jumpscareWithMiniGames.TriggerJumpscareSequence();
        });

        
    }
    
    private void Hide()
    {
        screamerImage.gameObject.SetActive(false);
        screamerPanel.SetActive(false);
        screamerImage.rectTransform.anchoredPosition = startPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player_Object")
        {
            TriggerJumpscare();
        }
    }
}