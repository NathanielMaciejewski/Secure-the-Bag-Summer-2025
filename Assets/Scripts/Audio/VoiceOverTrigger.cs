using UnityEngine;

public class VoiceOverTrigger : MonoBehaviour
{
    public static VoiceOverTrigger instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Debug.LogError("Found more than one Audio Manager in the scene.");
            Destroy(gameObject);
        }
    }
}
