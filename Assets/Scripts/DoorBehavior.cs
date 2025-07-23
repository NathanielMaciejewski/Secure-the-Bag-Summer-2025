using FMOD.Studio;
using UnityEngine;

public class DoorBehavior : OnOffSwitchReceiver
{
    [SerializeField] public bool isDoorOpen = false;
    [SerializeField] private bool opensOnSwitchDown = true;
    [SerializeField] private float trackLengthY = 3.0f;
    [SerializeField] private float trackLengthX = 0f;
    [SerializeField] private float animationSpeed = 10f;

    private Vector3 openPosition;
    private Vector3 closedPosition;
    private bool isAnimating = false;

    #region Audio
    private EventInstance doorOpen;
    private EventInstance doorClose;
    #endregion

    void Awake()
    {
        openPosition = transform.position + new Vector3(trackLengthX, trackLengthY);
        closedPosition = transform.position;

        if (isDoorOpen)
            isAnimating = true;
    }

    void Start()
    {
        doorOpen = AudioManager.instance.CreateEventInstance(FMODEvents.instance.mechanicalDoorOpen);
        doorClose = AudioManager.instance.CreateEventInstance(FMODEvents.instance.mechanicalDoorClose);
    }

    void Update()
    {
        if (isAnimating)
        {
            if (isDoorOpen)
            {
                transform.position = Vector3.MoveTowards(transform.position, openPosition, animationSpeed * Time.deltaTime);
                StopSound(doorClose);
                UpdateSound(doorOpen);
                if (transform.position == openPosition)
                    isAnimating = false;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, closedPosition, animationSpeed * Time.deltaTime);
                StopSound(doorOpen);
                UpdateSound(doorClose);
                if (transform.position == closedPosition)
                    isAnimating = false;
            }
        }
    }

    public override void OnSwitchDown()
    {
        isAnimating = true;
        isDoorOpen = opensOnSwitchDown;
    }

    public override void OnSwitchUp()
    {
        isAnimating = true;
        isDoorOpen = !opensOnSwitchDown;
    }

    private void UpdateSound(EventInstance doorEventInstance)
    {
        PLAYBACK_STATE playbackState;
        doorEventInstance.getPlaybackState(out playbackState);

        if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            doorEventInstance.start();
    }

    private void StopSound(EventInstance doorEventInstance)
    {
        PLAYBACK_STATE playbackState;
        doorEventInstance.getPlaybackState(out playbackState);

        if (playbackState.Equals(PLAYBACK_STATE.PLAYING))
            doorEventInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }

}