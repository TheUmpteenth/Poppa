///---------------------------------------------------------------------------------------------------------------------
/// Copyright Davie Farrell - 2021
///---------------------------------------------------------------------------------------------------------------------

using System;
using UnityEngine;

public interface ILaunchable
{
    public event Action OnLaunchableBecomesInactive;
    public event Action<ILaunchable> OnLaunchableDestroyed;
    
    GameObject View { get; }
    Vector2 Velocity { get; set; }
    float Speed { get; set; }
    
    //TODO: temporary - this screams at the need for a manager for tokens
    Canvas ParentCanvas { set; }
    
    void Launch(Vector2 velocity);
    void UpdateMovement();
}
