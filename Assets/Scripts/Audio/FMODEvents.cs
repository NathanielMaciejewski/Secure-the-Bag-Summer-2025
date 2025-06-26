using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference levelOneAmbience { get; private set; }


    [field: Header("Music")]
    [field: SerializeField] public EventReference score { get; private set; }

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference jump { get; private set; }
    [field: SerializeField] public EventReference defaultFootsteps { get; private set; }

    [field: Header("Bag SFX")]
    [field: SerializeField] public EventReference bagGrab { get; private set; }
    [field: SerializeField] public EventReference bagRelease { get; private set; }


    public static FMODEvents instance { get; private set; }

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
