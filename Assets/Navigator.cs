using UnityEngine;
using System.Collections;

public class Navigator : MonoBehaviour
{
    public Vector3 Destination;
    public float Speed;
    bool alreadySet;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Destination != Vector3.zero && alreadySet)
        {
            gameObject.MoveTowards(Destination, Speed);
        }

        alreadySet = false;
    }

    public void StayStill()
    {
        alreadySet = true;
        Destination = Vector3.zero;
    }

    public void SetDestination(Vector3 destination, float speed)
    {
        if (alreadySet) return;

        Destination = destination;
        Speed = speed;
        alreadySet = true;
    }
}
