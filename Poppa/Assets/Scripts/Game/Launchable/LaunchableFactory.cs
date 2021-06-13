///---------------------------------------------------------------------------------------------------------------------
/// Copyright Davie Farrell - 2021
///---------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaunchableFactory : MonoBehaviour
{
    //TODO: make these pools
    [SerializeField] private List<GameObject> spawnablePrefabs;
    [SerializeField] private Canvas m_canvas;

    private int m_numInScene = 0;
    
    public ILaunchable Create()
    {
        Debug.Assert(spawnablePrefabs.Count > 0, "[LaunchableFactory] No prefabs to choose from!");
        var rand = Mathf.FloorToInt(Random.value * spawnablePrefabs.Count);
        var spawnee = Instantiate(spawnablePrefabs[rand]);
        var launchable = spawnee.GetComponent<ILaunchable>();
        launchable.OnLaunchableDestroyed += Return;
        launchable.ParentCanvas = m_canvas;
        Debug.Assert(spawnee != null, $"[LaunchableFactory] {spawnablePrefabs[rand]} does not have a component that extends ILaunchable, and this is not allowed");
        ++m_numInScene;
        return launchable;
    }

    private void Return(ILaunchable launchable)
    {
        if (--m_numInScene <= 0)
        {
            SceneManager.LoadScene(SceneDefs.k_gameOverSceneIdx);
        }
    }
}
