////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// Copyright Davie Farrell - 2021
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using UnityEngine;

public class LaunchableFactory : MonoBehaviour
{
    //make these pools
    [SerializeField] private List<GameObject> spawnablePrefabs;
    [SerializeField] private Canvas m_canvas;
    
    public ILaunchable Create()
    {
        Debug.Assert(spawnablePrefabs.Count > 0, "[LaunchableFactory] No prefabs to choose from!");
        var rand = Mathf.FloorToInt(Random.value * spawnablePrefabs.Count);
        var spawnee = Instantiate(spawnablePrefabs[rand]);
        var launchable = spawnee.GetComponent<ILaunchable>();
        launchable.ParentCanvas = m_canvas;
        Debug.Assert(spawnee != null, $"[LaunchableFactory] {spawnablePrefabs[rand]} does not have a component that extends ILaunchable, and this is not allowed");
        return launchable;
    }
}
