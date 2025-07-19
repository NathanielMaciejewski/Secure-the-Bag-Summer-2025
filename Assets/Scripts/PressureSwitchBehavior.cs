using FMOD.Studio;
using UnityEngine;

public class PressureSwitchBehavior : MonoBehaviour
{
    private enum SwitchState
    {
        UNPRESSED,
        HALF_PRESSED,
        PRESSED,
    }

    [SerializeField] private OnOffSwitchReceiver target;
    [SerializeField] private float halfPressWeightThreshold = 1.0f;
    [SerializeField] private float fullPressWeightThreshold = 1.0f;
    [SerializeField] private bool shouldResetWhenUnpressed = true;
    [SerializeField] private bool loggingEnabled = false;
    [SerializeField] private LayerMask gravityLayer;

    private SwitchState switchState = SwitchState.UNPRESSED;
    private BoxCollider2D boxCollider;
    private Vector3 platePositionUp;
    private Vector3 platePositionHalf;
    private Vector3 platePositionDown;
    private bool isAnimating = false;
    private EventInstance switchDown;
    private EventInstance switchUp;

    void Start()
    {
        switchDown = AudioManager.instance.CreateEventInstance(FMODEvents.instance.buttonPress);
        switchUp = AudioManager.instance.CreateEventInstance(FMODEvents.instance.buttonRelease);
    }

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        platePositionUp = transform.position;
        platePositionDown = transform.position - new Vector3(0, transform.localScale.y / 2, 0);
        platePositionHalf = (platePositionUp + platePositionDown) / 2;
    }

    private void Update()
    {
        if (isAnimating)
        {
            switch (switchState)
            {
                case SwitchState.UNPRESSED:
                    Animate(platePositionUp);
                    break;
                case SwitchState.HALF_PRESSED:
                    Animate(platePositionHalf);
                    break;
                case SwitchState.PRESSED:
                    Animate(platePositionDown);
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
         RecalculatePressure(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
         RecalculatePressure(collision);
    }

    private void RecalculatePressure(Collider2D collision)
    {
        // If switch is pressed and shouldn't unpress, don't do anything
        if (switchState == SwitchState.PRESSED && !shouldResetWhenUnpressed)
            return;

        // If the trigger change wasn't something interesting, no need to update
        if (!collision.CompareTag("Player") && !collision.CompareTag("Grabbable"))
            return;

        // Get all things on the switch
        Collider2D[] thingsOnSwitch = Physics2D.OverlapBoxAll(boxCollider.bounds.center, boxCollider.bounds.size, 0, gravityLayer);

        float cumulativeWeight = 0;
        SwitchWeight thingWeight;

        if (loggingEnabled)
            Debug.Log("There are " + thingsOnSwitch.Length + " things on the switch");

        // Add up weight of all things on the switch
        for (int i = 0; i < thingsOnSwitch.Length; i++)
        {
            thingWeight = thingsOnSwitch[i].transform.GetComponent<SwitchWeight>();
            if (thingWeight != null)
                cumulativeWeight += thingWeight.GetTotalWeight();
        }

        if (loggingEnabled)
            Debug.Log("The things on the switch weigh " + cumulativeWeight);

        // Adjust state based on the amount of weight on the switch
        if (cumulativeWeight >= fullPressWeightThreshold)
            ChangeState(SwitchState.PRESSED);
        else if (cumulativeWeight >= halfPressWeightThreshold)
            ChangeState(SwitchState.HALF_PRESSED);
        else
            ChangeState(SwitchState.UNPRESSED);
    }

    private void ChangeState(SwitchState newState)
    {
        // If state is unchanged, do nothing
        if (switchState == newState)
            return;

        if (target != null)
        {
            // If the switch is now pressed
            if (newState == SwitchState.PRESSED)
                target.OnSwitchDown();

            // If the switch used to be pressed, thus now isn't
            if (switchState == SwitchState.PRESSED)
                target.OnSwitchUp();
        }

        switchState = newState;
        isAnimating = true;
    }

    private void Animate(Vector3 targetPosition)
    {
        if (transform.position.y < targetPosition.y)
            UpdateSound(switchUp);
        else
            UpdateSound(switchDown);

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime);

        if (transform.position == targetPosition)
            isAnimating = false;
    }

    private void UpdateSound(EventInstance doorEventInstance)
    {
        PLAYBACK_STATE playbackState;
        doorEventInstance.getPlaybackState(out playbackState);

        if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            doorEventInstance.start();
    }
}
