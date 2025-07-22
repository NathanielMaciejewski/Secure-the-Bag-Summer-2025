using UnityEditor.SearchService;
using UnityEngine;

public class SingleRunBehavior : MonoBehaviour
{
    [SerializeField] private SingleRunManager manager;
    [SerializeField] private int id;

    void Start()
    {
        if (manager != null)
        {
            manager = manager.Validate();
            manager.Register(id);
        }
    }

    protected void SetUsed()
    {
        if (manager != null)
            manager.Deactivate(id);
    }

    protected bool IsActive()
    {
        if (manager == null)
        {
            Debug.Log("Attempting to fire trigger " + id + " but manager is null");
            return false;
        }

        Debug.Log("Attempting to fire trigger " + id + ", manager says " + manager.IsActive(id));
        return manager.IsActive(id);
    }
}
