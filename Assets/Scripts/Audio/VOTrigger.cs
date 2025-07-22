using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class VOTrigger : MonoBehaviour
{
    [field: Header("Parameter Change")]
    [field: SerializeField] public EventReference voicOverLine { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag.Equals("Player"))
        {
            RuntimeManager.PlayOneShot(voicOverLine);
            Destroy(gameObject);
        }
    }
}
