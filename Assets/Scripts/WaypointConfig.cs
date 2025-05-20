using UnityEngine;
using System.Collections.Generic;

public static class WaypointConfig
{
    private static readonly Dictionary<string, float> waypointWaitTimes = new Dictionary<string, float>
    {
        { "Kitchen_Waypoint", 4f },
        { "Corridor_Waypoint", 3f },
        { "Bedroom_Waypoint", 3f },
        { "Attic_Waypoint", 2f },
        { "Bathroom_Waypoint", 6f },
        { "Second_Floor_Waypoint", 3f },
        { "Attic_Room_Waypoint", 3f }
    };

    public static float GetWaitTime(Transform waypoint)
    {
        if (waypoint != null && waypointWaitTimes.TryGetValue(waypoint.name, out float waitTime))
        {
            return waitTime;
        }
        Debug.LogWarning($"No wait time defined for waypoint {waypoint?.name}. Using default: 3f");
        return 3f;
    }
}