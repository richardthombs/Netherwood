using UnityEngine;
using System.Collections;

public class RandomWalk : MonoBehaviour
{
    public Vector3 Destination;
    public float Duration;
    public float Speed;

    float timeForNewDestination;
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

        if (Destination == Vector3.zero || transform.position.IsCloseTo(Destination) || Time.time > timeForNewDestination)
        {
            Destination = MainObject.RandomPosition(1f);
            timeForNewDestination = Time.time + Duration;
        }
        else
        {
            nav.SetDestination(Destination, Speed);
        }
    }
}
