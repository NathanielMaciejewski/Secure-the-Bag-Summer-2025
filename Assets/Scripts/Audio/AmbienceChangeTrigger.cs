using UnityEngine;

public class AmbienceChangeTrigger : MonoBehaviour
{
    [Header("Area")]
    [SerializeField] private AmbienceArea area;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag.Equals("Player"))
        {
            AudioManager.instance.SetAmbienceArea(area);
        }
    }
}
