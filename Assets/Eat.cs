using UnityEngine;
using System.Collections;

public class Eat : MonoBehaviour
{
    public float Speed;
    public Food Food;
    public Vector3 Destination;
    Metab metabolism;
    Senses senses;
    Navigator nav;

    // Use this for initialization
    void Start()
    {

    }

    void Awake()
    {
        metabolism = GetComponent<Metab>();
        senses = GetComponent<Senses>();
        nav = GetComponent<Navigator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (metabolism == null || senses == null || nav == null) return;
        if (!metabolism.Hungry) return;

        // Pick the first food we see
        if (Food == null && senses.Food.Count > 0) Food = senses.Food[0];

        if (Food != null)
        {
            if (transform.position.IsCloseTo(Food.transform.position))
            {
                // Eat
                var eaten = Food.Eat(metabolism.Stomach.Capacity - metabolism.Stomach.Contents);
                metabolism.Stomach.Ingest(eaten);
                if (eaten <= 0) Food = null;
            }
            else
            {
                nav.SetDestination(Food.transform.position, Speed);
            }
        }
        else
        {
            if (Destination == Vector3.zero || transform.position.IsCloseTo(Destination)) Destination = MainObject.RandomPosition(1f);
            else
            {
                nav.SetDestination(Destination, Speed);
            }
        }
    }
}
