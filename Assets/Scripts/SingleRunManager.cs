using UnityEngine;
using System.Collections.Generic;

public class SingleRunManager : MonoBehaviour
{
    public static SingleRunManager instance;

    private Dictionary<int, bool> flags = new Dictionary<int, bool>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        if (this != instance)
            Destroy(gameObject);
    }

    public SingleRunManager Validate()
    {
        return instance;
    }

    public void Register(int id)
    {
        if (!flags.ContainsKey(id))
            flags.Add(id, true);
    }

    public void Deactivate(int id)
    {
        if (flags.ContainsKey(id))
            flags[id] = false;
    }

    public bool IsActive(int id)
    {
        if (!flags.ContainsKey(id))
            return false;
        return flags[id];
    }
}
