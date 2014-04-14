using UnityEngine;
using System.Collections;

public static class Vector3Extensions
{
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

public class RandomWalk : MonoBehaviour
{
    public Vector3 Destination;
    public float Speed;
    Navigator nav;

    void Start()
    {
        transform.position = transform.position.PlaceOnTerrain(1f);
    }

    void Awake()
    {
        nav = GetComponent<Navigator>();
    }

    // Update is called once per frame
    void Update()
    {
       // if (death != null && death.Dead) return;

        if (Destination == Vector3.zero || transform.position.IsCloseTo(Destination)) Destination = MainObject.RandomPosition(1f);
        else
        {
            nav.SetDestination(Destination, Speed);
        }
    }
}
