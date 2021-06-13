////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// Copyright Davie Farrell - 2021
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

public interface ILaunchable
{
    GameObject View { get; }
    Vector2 Velocity { get; set; }
    float Speed { get; set; }
    
    //temporary - this screams at the need for a manager for tokens
    Canvas ParentCanvas { set; }
    
    void Launch(Vector2 velocity);
    void UpdateMovement();
}
