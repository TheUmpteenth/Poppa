////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// Copyright Davie Farrell - 2021
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherController : MonoBehaviour
{
    [SerializeField] private RectTransform m_ballLocator;
    [SerializeField] private RectTransform m_viewRect;
    
    private Vector3 m_eulerAngles = Vector3.zero;
    private Vector2 m_locatorPos = Vector2.zero;
    private Vector2 m_pointerPos = Vector2.zero;

    void Update()
    {
        FollowMouse();
    }
    
    private void FollowMouse()
    {
        var launcherPos = m_ballLocator.position;
        var pointerPos = Input.mousePosition;
        
        m_locatorPos.x = launcherPos.x;
        m_locatorPos.y = launcherPos.z;
        m_pointerPos.x = pointerPos.x;
        m_pointerPos.y = pointerPos.y;
        
        var vectorBetween = m_pointerPos - m_locatorPos;

        m_eulerAngles.z = -Vector2.SignedAngle(vectorBetween.normalized, Vector2.up);
        m_ballLocator.eulerAngles = m_eulerAngles;
    }
}
