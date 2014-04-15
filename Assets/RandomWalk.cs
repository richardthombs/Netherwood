using UnityEngine;
using System.Collections;

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
