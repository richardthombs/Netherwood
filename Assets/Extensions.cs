using UnityEngine;
using System.Collections;

public static class Vector3Extensions
{
    public static float DistanceTo(this Vector3 me, Vector3 position)
    {
        return Vector3.Distance(me, position);
    }

    public static bool IsCloseTo(this Vector3 me, Vector3 position)
    {
        return (Vector3.Distance(me, position) < 1.5f);
    }

    public static Vector3 PlaceOnTerrain(this Vector3 me, float yOffset)
    {
        var newPos = me;
        newPos.y = Terrain.activeTerrain.SampleHeight(me) + yOffset;
        return newPos;
    }
}

public static class GameObjectExtensions
{

    public static void MoveTowards(this GameObject me, Vector3 destination, float speed)
    {
        me.transform.LookAt(destination);
        var newPos = me.transform.position + me.transform.forward * speed * Time.deltaTime;

        var yOffset = me.transform.position.y - Terrain.activeTerrain.SampleHeight(me.transform.position);
        me.transform.position = newPos.PlaceOnTerrain(yOffset);
    }
}
