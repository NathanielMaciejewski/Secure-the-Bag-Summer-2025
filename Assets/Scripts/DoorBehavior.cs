using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DoorBehavior : MonoBehaviour
{
    public bool _isDoorOpen = false;
    Vector3 _doorClosedPos;
    Vector3 _doorOpenPos;
    float _doorSpeed = 10f;

    #region Audio
    private EventInstance doorOpen;
    private EventInstance doorClose;
    #endregion

    void Awake()
    {
        _doorClosedPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        _doorOpenPos = new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z);
    }

    void Start()
    {
        doorOpen = AudioManager.instance.CreateEventInstance(FMODEvents.instance.mechanicalDoorOpen);
        doorClose = AudioManager.instance.CreateEventInstance(FMODEvents.instance.mechanicalDoorClose);
    }

    void Update()
    {
        if (_isDoorOpen)
        {
            OpenDoor();
        }
        else if (!_isDoorOpen)
        {
            CloseDoor();
        }
    }

    void OpenDoor()
    {
        if (transform.position != _doorOpenPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, _doorOpenPos, _doorSpeed * Time.deltaTime);
            UpdateSound(doorOpen);
        }
    }

    void CloseDoor()
    {
        if (transform.position != _doorClosedPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, _doorClosedPos, _doorSpeed * Time.deltaTime);
            UpdateSound(doorClose);
        }
    }

    private void UpdateSound(EventInstance doorEventInstance)
    {
        PLAYBACK_STATE playbackState;
        doorEventInstance.getPlaybackState(out playbackState);

        if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                doorEventInstance.start();
            }
    }

}