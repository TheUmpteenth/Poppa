///---------------------------------------------------------------------------------------------------------------------
/// Copyright Davie Farrell - 2021
///---------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

public interface IAttachable
{
    List<IAttachable> AttachedItems { get; set; }
    Collider2D Collider { get; }
    
    void BeginAttaching(IAttachable other);
    void ResolveAttachments();
    void SetAttachment(IAttachable other);
    Vector2 GetAttachedPosition(Collision2D collision);
}
