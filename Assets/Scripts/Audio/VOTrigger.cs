using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class VOTrigger : MonoBehaviour
{
    [field: Header("Parameter Change")]
    [field: SerializeField] public EventReference voicOverLine { get; private set; }

    private bool hasPlayed;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        hasPlayed = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (hasPlayed == false)
        {
            if (collider.tag.Equals("Player"))
            {
                RuntimeManager.PlayOneShot(voicOverLine);
            }
        }
        hasPlayed = true;
        return;

    }
}
