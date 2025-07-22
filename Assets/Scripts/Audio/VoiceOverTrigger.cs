using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class VoiceOverTrigger : SingleRunBehavior
{
    [field: Header("Voice Over Line")]
    [field: SerializeField] public EventReference voicOverLine { get; private set; }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (IsActive() && collider.CompareTag("Player"))
        {
            RuntimeManager.PlayOneShot(voicOverLine);
            SetUsed();
        }
    }
}
