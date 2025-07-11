using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DoorSwitchBehavior : MonoBehaviour
{
    [SerializeField] DoorBehavior _doorBehavior;

    [SerializeField] bool isDoorOpenSwitch;
    [SerializeField] bool isDoorCloseSwitch;

    float _switchSizeY;
    Vector3 _switchUpPos;
    Vector3 _switchDownPos;
    float _switchSpeed = 1f;
    float _switchDelay = 0.2f;
    bool isPressingSwitch = false;

    // Start is called before the first frame update
    void Awake()
    {
        _switchSizeY = gameObject.transform.localScale.y / 2;
        _switchUpPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        _switchDownPos = new Vector3(transform.position.x, transform.position.y - _switchSizeY, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPressingSwitch)
        {
            MoveSwitchDown();
        }
        else if (!isPressingSwitch)
        {
            MoveSwitchUp();
        }
    }

    void MoveSwitchDown()
    {
        if (transform.position != _switchDownPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, _switchDownPos, _switchSpeed * Time.deltaTime);
        }
    }

    void MoveSwitchUp()
    {
        if (transform.position != _switchUpPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, _switchUpPos, _switchSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || (collision.CompareTag("Grabbable")))
        {
            isPressingSwitch = !isPressingSwitch;

            if (isDoorOpenSwitch && !_doorBehavior._isDoorOpen)
            {
                _doorBehavior._isDoorOpen = !_doorBehavior._isDoorOpen;
            }
            //else if (isDoorCloseSwitch && _doorBehavior._isDoorOpen)
            //{
            //    _doorBehavior._isDoorOpen = !_doorBehavior._isDoorOpen;
            //}
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || (collision.CompareTag("Grabbable")))
        {
            
            if (isDoorCloseSwitch && _doorBehavior._isDoorOpen)
            {
                _doorBehavior._isDoorOpen = !_doorBehavior._isDoorOpen;
            }
            StartCoroutine(SwitchUpDelay(_switchDelay));
        }
    }

    IEnumerator SwitchUpDelay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isPressingSwitch = false;
    }
}

