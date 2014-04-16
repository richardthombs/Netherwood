using UnityEngine;
using System.Collections.Generic;

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
public class BodyX
{
    public float Health;

    public Stomach Stomach;
    public FatReserve Fat;
    public Metabolism Metabolism;

    public bool Hungry;
    public bool Dead;

    public float Age;
    public float LifeExpectancy;

    public void Tick(float elapsed)
    {
        Age += elapsed;

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

[System.Serializable]
public class ReproductionX
{
    public Sex Sex;
    public float GestationPeriod = 30;
    public float SeasonInterval = 120;
    public float SeasonDuration = 30;

    public bool LookingForMate;
    public bool Pregnant;
    public float DueDate;
    public bool Due;

    public Color Species;

    public void Tick(float elapsed)
    {
        Due = (Pregnant && DueDate > 0 && Time.time > DueDate);

        if (Pregnant && DueDate == 0) DueDate = Time.time + GestationPeriod;

        LookingForMate = IsMatingSeason(Time.time) && !Pregnant;

        if (LookingForMate && Sex == Sex.Asexual) Pregnant = true;
    }

    public void GiveBirth(Mobile parent)
    {
        Pregnant = false;
        DueDate = 0;
        Due = false;

        var baby = (Mobile)GameObject.Instantiate(parent);
        baby.Reproduction.Species = MutateSpecies(parent.Reproduction.Species, parent.Reproduction.Species, 0.05f);
        baby.renderer.material.color = baby.Reproduction.Species;
        baby.Body.Stomach.Contents = parent.Body.Fat.Get(parent.Body.Fat.Contents / 2);
        baby.Body.Fat.Contents = 0;
        baby.Body.Age = 0;
        baby.LastFood = null;
        if (parent.Reproduction.Sex != Sex.Asexual) baby.Reproduction.Sex = (Sex)Random.Range(1, 2);
    }

    public bool IsMatingSeason(float time)
    {
        float start = Mathf.Floor(time / SeasonInterval) * SeasonInterval;
        float end = start + SeasonDuration;

        return start <= time && time < end;
    }

    public bool IsSuitableMate(ReproductionX candidate)
    {
        if (Sex == Sex.Asexual && candidate.Sex != Sex.Asexual) return false;
        if (Sex != Sex.Asexual && candidate.Sex == Sex.Asexual) return false;
        if (Sex == candidate.Sex) return false;

        return SpeciesMatch(Species, candidate.Species, 0.1f);
    }

    public bool IsSameSpecies(Mobile candidate)
    {
        return SpeciesMatch(Species, candidate.Reproduction.Species, 0.1f);
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
        var r = Mathf.Clamp(((s1.r + s2.r) / 2) + Random.value * mutation, 0, 1);
        var g = Mathf.Clamp(((s1.g + s2.g) / 2) + Random.value * mutation, 0, 1);
        var b = Mathf.Clamp(((s1.b + s2.b) / 2) + Random.value * mutation, 0, 1);

        return new Color(r, g, b);
    }
}

public class Mobile : MonoBehaviour
{
    public Vector3 Destination;

    public BodyX Body = new BodyX
    {
        Stomach = new Stomach { Capacity = 100, Contents = 0 },
        Fat = new FatReserve { Capacity = float.MaxValue, Contents = 0 },
        Metabolism = new Metabolism { FatConversionRate = 1, FoodConversionRate = 1 }

    };

    public ReproductionX Reproduction = new ReproductionX
    {
    };

    public GameObject LastFood;
    public float Speed = 0.1f;
    public float ViewRange = 10;
    public float Lonliness;

    int nextMinute = 0;

    void MinuteTick()
    {
        if (Body.Dead) return;

        if (Reproduction.Due)
        {
            Reproduction.GiveBirth(this);
            return;
        }

        LookAround();

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
        //renderer.material.color = Color.black;
        rigidbody.isKinematic = false; // So we fall over and roll down hill :)
        Destroy(gameObject, 30);
    }

    void LookAround()
    {
        var colliders = Physics.OverlapSphere(transform.position, ViewRange);
        List<Food> food = new List<Food>();
        List<Mobile> mobiles = new List<Mobile>();

        foreach (var col in colliders)
        {
            if (col.gameObject == gameObject) continue;

            var f = col.GetComponent<Food>();
            if (f != null) food.Add(f);

            var m = col.GetComponent<Mobile>();
            if (m != null) mobiles.Add(m);

            OnObjectSeen(col);
        }

        foreach (var f in food)
        {
            f.renderer.material.color = Color.red;
        }
    }

    void Tick()
    {
        if (Body.Dead) return;

        Body.Tick(Time.deltaTime);

        if (Body.Health <= 0 || Body.Age > Body.LifeExpectancy)
        {
            Die();
            return;
        }

        Reproduction.Tick(Time.deltaTime);

        Lonliness += Time.deltaTime;

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

    void OnObjectSeen(Collider col)
    {
        var f = col.gameObject.GetComponent<Food>();
        if (f != null) OnFoodSeen(f);

        var m = col.gameObject.GetComponent<Mobile>();
        if (m != null) OnMobileSeen(m);
    }

    void OnFoodSeen(Food f)
    {
        // If there is no food left, ignore it
        if (f.FoodRemaining <= 0) return;

        // IF this is closer than the last food we saw, pick it
        LastFood = Closest(LastFood, f.gameObject);
    }

    void OnMobileSeen(Mobile m)
    {
        if (Reproduction.IsSameSpecies(m))
        {
            Lonliness = 0;
        }
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
