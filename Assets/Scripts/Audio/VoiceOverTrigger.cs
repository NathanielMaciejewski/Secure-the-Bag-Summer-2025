using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class VoiceOverTrigger : MonoBehaviour
{
    [field: Header("Voice Over Line")]
    [field: SerializeField] public EventReference voicOverLine { get; private set; }

    private bool hasPlayed;

    private void Awake()
    {
        hasPlayed = false;
        Debug.Log("New VO Trigger Created" + gameObject.name);
        Debug.Log("Has Played? " + hasPlayed);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag.Equals("Player"))
        {
            if (hasPlayed == false)
            {
                RuntimeManager.PlayOneShot(voicOverLine);
                hasPlayed = true;
            }
            Debug.Log("Has Played? " + hasPlayed);
        }
        else return;

    }
}
