///---------------------------------------------------------------------------------------------------------------------
/// Copyright Davie Farrell - 2021
///---------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

public interface IMatchable
{
    MatchDefs.MatchType Type { get; set; }
    Color Colour { get; set; }
    
    bool TryGetMatchedGroup(ref List<IMatchable> matchedGroup);
    void SetMatchType(MatchDefs.MatchType type);
    bool IsMatched(MatchDefs.MatchType type);
    void SetMatchComplete();
}
