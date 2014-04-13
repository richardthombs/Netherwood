using UnityEngine;
using System.Collections;

public class Mobile : MonoBehaviour
{
    public Vector3 Destination;
    public float forwardForce = 3f;

    public float Energy;
    public float Health;
    public bool Dead;
    public bool Hungry;
    public GameObject LastFood;
    public float Speed = 0.1f;

    int nextMinute = 0;

    void MinuteTick()
    {
        if (Dead) return;

        if (Energy > 0) Energy--;
        else Health--;

        if (Health <= 0)
        {
            Die();
            return;
        }

        Hungry = Energy < 50;

        if (Hungry)
        {
            if (LastFood != null) Destination = LastFood.transform.position;
        }

        if (Arrived())
        {
            if (Hungry && LastFood != null)
            {
                var eaten = LastFood.GetComponent<Food>().Eat(10f);
                Energy += eaten;
                if (eaten <= 0)
                {
                    LastFood = null;
                    Destination = MainObject.RandomPosition(1f);
                }
            }
            else
            {
                Destination = MainObject.RandomPosition(1f);
            }
        }
    }

    private void Die()
    {
        Dead = true;
        renderer.material.color = Color.black;
        rigidbody.isKinematic = false; // So we fall over and roll down hill :)
        Destroy(gameObject, 30);
    }

    void Tick()
    {
        if (Dead) return;

        transform.LookAt(Destination);

        if (!Arrived())
        {
            //rigidbody.AddForce(transform.forward * forwardForce);

            var newPos = PlaceOnTerrain(transform.position + transform.forward * Speed * Time.deltaTime, 1f);
            transform.position = newPos;
        }
    }

    Vector3 PlaceOnTerrain(Vector3 position, float yOffset)
    {
        var newPos = position;
        newPos.y = Terrain.activeTerrain.SampleHeight(position) + yOffset;
        return newPos;
    }

    bool Arrived()
    {
        return (Vector3.Distance(transform.position, Destination) <= 1.5);
    }

	// Use this for initialization
	void Start () {
        Destination = MainObject.RandomPosition();
	}

    void Update()
    {
        if (Time.time > nextMinute)
        {
            MinuteTick();
            nextMinute += 1;
        }

        Tick();
    }

    void OnTriggerEnter(Collider col)
    {
        var f = col.gameObject.GetComponent<Food>();

        // If this wasn't food, or there was none left, ignore it
        if (f == null || f.FoodRemaining <= 0) return;

        // IF this is closer than the last food we saw, pick it
        LastFood = Closest(LastFood, col.gameObject);
    }

    GameObject Closest(GameObject o1, GameObject o2)
    {
        if (o1 == null) return o2;
        if (o2 == null) return o1;

        var d1 = Vector3.Distance(transform.position, o1.transform.position);
        var d2 = Vector3.Distance(transform.position, o2.transform.position);

        if (d1 > d2) return o2;
        return o1;
    }
}
