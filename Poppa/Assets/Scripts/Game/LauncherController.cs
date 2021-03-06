///---------------------------------------------------------------------------------------------------------------------
/// Copyright Davie Farrell - 2021
///---------------------------------------------------------------------------------------------------------------------

using UnityEngine;

public class LauncherController : MonoBehaviour
{
    private const int k_specialThresholdMin = 5;
    private const int k_specialThresholdMax = 15;
    
    [SerializeField] private RectTransform m_ballLocator;
    [SerializeField] private LaunchableFactory m_launchableFactory;

    private ILaunchable m_loadedLaunchable;
    
    private Vector3 m_eulerAngles = Vector3.zero;
    private Vector2 m_locatorPos = Vector2.zero;
    private Vector2 m_pointerPos = Vector2.zero;
    private Vector2 m_launchVector = Vector2.zero;

    private int m_reloadCount = 0;
    private int m_specialThreshold = 5;

    private bool m_inputBusy;

    void Start()
    {
        Reload();
    }
    
    void Update()
    {
        FollowMouse();

        if (!m_inputBusy && Input.GetMouseButtonDown(0))
        {
            m_inputBusy = true;
            LaunchLoadedLaunchable();
        }
    }
    
    private void FollowMouse()
    {
        var launcherPos = m_ballLocator.position;
        var pointerPos = Input.mousePosition;
        
        m_locatorPos.x = launcherPos.x;
        m_locatorPos.y = launcherPos.z;
        m_pointerPos.x = pointerPos.x;
        m_pointerPos.y = pointerPos.y;
        
        m_launchVector = (m_pointerPos - m_locatorPos).normalized;

        m_eulerAngles.z = -Vector2.SignedAngle(m_launchVector, Vector2.up);
        m_ballLocator.eulerAngles = m_eulerAngles;
    }

    private void OnLaunchableBecomesInactive()
    {
        m_loadedLaunchable.OnLaunchableBecomesInactive -= OnLaunchableBecomesInactive;
        m_inputBusy = false;
        Reload();
    }

    private ILaunchable SpawnLaunchable(bool special)
    {
        var spawnee = m_launchableFactory.Create();
        if (spawnee is IMatchable matchable)
        {
            if (special)
            {
                matchable.SetMatchType(MatchDefs.RandomSpecialMatchType);
            }
            else
            {
                matchable.SetMatchType(MatchDefs.RandomMatchType);
            }
        }
        spawnee.View.transform.SetParent(m_ballLocator, true);
        spawnee.View.transform.SetPositionAndRotation(m_ballLocator.transform.position, Quaternion.identity);
        return spawnee;
    }

    private void LaunchLoadedLaunchable()
    {
        m_loadedLaunchable.OnLaunchableBecomesInactive += OnLaunchableBecomesInactive;
        m_loadedLaunchable?.Launch(m_launchVector);
    }

    private void Reload()
    {
        var special = false;
        if (++m_reloadCount > m_specialThreshold)
        {
            special = true;
            m_reloadCount = 0;
            m_specialThreshold = Mathf.FloorToInt(Random.value * (k_specialThresholdMax - k_specialThresholdMin) + k_specialThresholdMin);
        }
        m_loadedLaunchable = SpawnLaunchable(special);
        
    }
}
