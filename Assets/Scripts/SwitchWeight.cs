using UnityEngine;

public class SwitchWeight : MonoBehaviour
{
    [SerializeField] private float weight = 1.0f;

    public float GetTotalWeight()
    {
        SwitchWeight child = null;

        for (int i = 0; i < transform.childCount && child == null; i++)
        {
            child = transform.GetChild(i).GetComponent<SwitchWeight>();
        }

        if (child == null)
            return weight;

        return weight + child.GetTotalWeight();
    }
}
