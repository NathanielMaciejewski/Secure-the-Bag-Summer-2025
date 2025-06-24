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


    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene.");
        }
        instance = this;
    }

}
