using UnityEngine;
using System.Collections.Generic;

public class MainObject : MonoBehaviour
{
    public int FoodCount=500;
    public int MobCount=1;
    public static int Area = 500;

    static System.Random rnd = new System.Random();
    List<GameObject> mobs = new List<GameObject>();
    List<GameObject> food = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        var mobPrefab = GameObject.Find("Capsule");
        if (mobPrefab != null) CloneMob(mobPrefab, MobCount - 1);

        var foodPrefab = GameObject.Find("Food");
        for (int i = 0; i < FoodCount - 1; i++)
        {
            var obj = (GameObject)Instantiate(foodPrefab);
            obj.transform.position = RandomPosition(0);

            var f = obj.GetComponent<Food>();
            f.FoodRemaining = rnd.Next(1, 10) * 10;

            food.Add(obj);
        }
    }

    private void CloneMob(GameObject mobPrefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var obj = (GameObject)Instantiate(mobPrefab);
            obj.transform.position = RandomPosition(1f);
            obj.renderer.material.color = new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());

            var m = obj.GetComponent<Mobile>();
            if (m != null)
            {
                m.Body.Health = rnd.Next(1, 10) * 10;
                m.Destination = RandomPosition(1f);

                m.Reproduction.Sex = Sex.Asexual; // (Sex)rnd.Next(3);
                m.Reproduction.Species = obj.renderer.material.color;
            }

            mobs.Add(obj);
        }
    }

    public static Vector3 RandomPosition(float yOffset = 0f)
    {
        var newPos = new Vector3(rnd.Next(-Area, Area), 0, rnd.Next(-Area, Area));
        newPos.y = Terrain.activeTerrain.SampleHeight(newPos) + yOffset;
        return newPos;
    }
}

