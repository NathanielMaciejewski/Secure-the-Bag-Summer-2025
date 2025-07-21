using UnityEngine;

public class SwitchWeight : MonoBehaviour
{
    [SerializeField] private float weight = 1.0f;

    public float GetTotalWeight()
    {
        SwitchWeight child = null;
        float cumulativeWeight = weight;

        for (int i = 0; i < transform.childCount; i++)
        {
            child = transform.GetChild(i).GetComponent<SwitchWeight>();

            if (child != null)
                cumulativeWeight += child.GetTotalWeight();
        }

        return cumulativeWeight;
    }
}
