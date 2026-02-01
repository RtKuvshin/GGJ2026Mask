using System;
using UnityEngine;
using UnityEngine.Events;

public class ColliderBehinder : MonoBehaviour
{
    [Header("Custom Event")]
    public UnityEvent onTriggerEvent;

    [SerializeField] private TeleportMonologueTrigger TeleportMonologueTrigger;
    
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player") && TeleportMonologueTrigger.triggered)
        {
            // Call the custom event
            onTriggerEvent?.Invoke();
        }
    }
}
