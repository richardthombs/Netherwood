using UnityEngine;
using System.Collections;

[System.Serializable]
public class Stomach
{
    public float Capacity;
    public float Contents;

    public float Digest(float qty)
    {
        var digested = Mathf.Min(qty, Contents);
        Contents -= digested;
        return digested;
    }

    public float Ingest(float qty)
    {
        var ingested = Mathf.Min(Capacity - Contents, qty);
        Contents += ingested;
        return ingested;
    }
}

[System.Serializable]
public class FatReserve
{
    public float Capacity;
    public float Contents;

    public float Get(float qty)
    {
        var digested = Mathf.Min(qty, Contents);
        Contents -= digested;
        return digested;
    }

    public float Put(float qty)
    {
        var ingested = Mathf.Min(Capacity - Contents, qty);
        Contents += ingested;
        return ingested;
    }
}

[System.Serializable]
public class Metabolism
{
    public float FoodConversionRate;
    public float FatConversionRate;
    public float BurnRate;
}

[System.Serializable]
public class Body
{
    public float Health;

    public Stomach Stomach;
    public FatReserve Fat;
    public Metabolism Metabolism;

    public bool Hungry;
    public bool Dead;

    public void Tick(float elapsed)
    {
        float energyRequired = Metabolism.BurnRate * elapsed;

        // Digest our food
        var foodConverted = Metabolism.FoodConversionRate * elapsed;
        var energyFromFood = Stomach.Digest(foodConverted);
        energyRequired -= energyFromFood;

        // If we have an energy shortfall, then we digest some fat too
        if (energyRequired > 0)
        {
            var fatConverted = Metabolism.FatConversionRate * elapsed;
            var energyFromFat = Fat.Get(fatConverted);
            energyRequired -= energyFromFat;
        }

        if (energyRequired < 0)
        {
            // If we ended up with an energy surplus, store it as fat
            Fat.Put(-energyRequired);
        }
        else
        {
            // If we don't have enough energy, our health suffers
            Health -= energyRequired;
        }

        // We're hungry if our stomach is less than 5% full
        Hungry = (Stomach.Contents < Stomach.Capacity * 0.05);
    }
}

public enum Sex
{
    Male,
    Female,
    Asexual
}

[System.Serializable]
public class Reproduction
{
    public Sex Sex;
    public bool LookingForMate;
    public float SeasonStart;
    public float SeasonDuration;
    public float SeasonEnd;
    public float SeasonInterval;

    public Color Species;

    public void Tick(float elapsed)
    {
        if (Time.time >= SeasonStart)
        {
            LookingForMate = true;
            SeasonEnd = Time.time + SeasonDuration;
        }

        if (Time.time >= SeasonEnd)
        {
            LookingForMate = false;
            SeasonStart = Time.time + SeasonInterval;
        }
    }

    public bool IsSuitableMate(Reproduction candidate)
    {
        if (Sex == Sex.Asexual && candidate.Sex != Sex.Asexual) return false;
        if (Sex != Sex.Asexual && candidate.Sex == Sex.Asexual) return false;
        if (Sex == candidate.Sex) return false;

        return (SpeciesMatch(Species, candidate.Species, 0.1f));
    }

    static bool SpeciesMatch(Color s1, Color s2, float tolerance)
    {
        var r = Mathf.Abs(s1.r - s2.r);
        var g = Mathf.Abs(s1.g - s2.g);
        var b = Mathf.Abs(s1.b - s2.b);

        return r <= tolerance && g <= tolerance && b <= tolerance;
    }

    static Color MutateSpecies(Color s1, Color s2, float mutation)
    {
        var r = (s1.r + s2.r) / 2;
        var g = (s1.g + s2.g) / 2;
        var b = (s1.b + s2.b) / 2;

        return new Color(r, g, b);
    }
}

public class Mobile : MonoBehaviour
{
    public Vector3 Destination;

    public Body Body = new Body
    {
        Stomach = new Stomach { Capacity = 100, Contents = 0 },
        Fat = new FatReserve { Capacity = float.MaxValue, Contents = 0 },
        Metabolism = new Metabolism { FatConversionRate = 1, FoodConversionRate = 1 }
    };

    public Reproduction Reproduction = new Reproduction
    {
    };

    public GameObject LastFood;
    public float Speed = 0.1f;


    int nextMinute = 0;

    void MinuteTick()
    {
        if (Body.Dead) return;

        if (Body.Hungry)
        {
            if (LastFood != null) Destination = LastFood.transform.position;
        }

        if (Arrived())
        {
            if (Body.Hungry && LastFood != null)
            {
                var eaten = LastFood.GetComponent<Food>().Eat(Body.Stomach.Capacity-Body.Stomach.Contents);
                Body.Stomach.Ingest(eaten);
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
        Body.Dead = true;
        renderer.material.color = Color.black;
        rigidbody.isKinematic = false; // So we fall over and roll down hill :)
        Destroy(gameObject, 30);
    }

    void Tick()
    {
        if (Body.Dead) return;

        Body.Tick(Time.deltaTime);

        if (Body.Health <= 0)
        {
            Die();
            return;
        }

        Reproduction.Tick(Time.deltaTime);

        transform.LookAt(Destination);

        if (!Arrived())
        {
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
