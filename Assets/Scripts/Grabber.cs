using UnityEngine;

public class Grabber : MonoBehaviour
{
    [SerializeField] private LayerMask grabLayer;

    private GameObject heldItem;

    public void Grab(GameObject thing)
    {
        if (heldItem != null || thing == null)
            return;

        if (thing.TryGetComponent<GrabbableBehavior>(out var target))
        {
            if (!target.IsGrabbable())
                return;

            heldItem = thing;
            target.Grab();
            PlayGrabSound();
            heldItem.transform.SetParent(transform);
            heldItem.transform.localPosition = new Vector3(0, 8, 0);
        }
    }

    public void Drop()
    {
        if (heldItem == null)
            return;

        heldItem.GetComponent<GrabbableBehavior>().Release(Vector3.zero);
        heldItem.transform.SetParent(null);
        heldItem = null;
        PlayReleaseSound();
    }

    public void Throw(Vector3 velocity)
    {
        if (heldItem == null)
            return;

        heldItem.GetComponent<GrabbableBehavior>().Release(velocity);
        heldItem.transform.SetParent(null);
        heldItem = null;
        PlayReleaseSound();
    }

    public bool HasItem()
    {
        return heldItem != null;
    }

    public bool IsEncumbered(float carryCapacity = 0)
    {
        if (heldItem == null)
            return false;

        if (heldItem.GetComponent<SwitchWeight>() == null)
            return true;

        return heldItem.GetComponent<SwitchWeight>().GetTotalWeight() >= carryCapacity;
    }

    public LayerMask GetGrabLayer()
    {
        return grabLayer;
    }

    private void PlayGrabSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.bagGrab, this.transform.position);
    }

    private void PlayReleaseSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.bagRelease, this.transform.position);
    }
}
