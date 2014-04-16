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
        var mobPrefab = GameObject.Find("Female");
        var mobRepro = mobPrefab.GetComponent<Reproduction>();
        mobRepro.Sex = Sex.Female;
        mobRepro.Pregnant = true;
        mobRepro.LitterSize = 4;

        if (mobPrefab != null) CloneMob(mobPrefab, MobCount - 1);

        var foodPrefab = GameObject.Find("Food");
        for (int i = 0; i < FoodCount - 1; i++)
        {
            var obj = (GameObject)Instantiate(foodPrefab);
            obj.transform.position = RandomPosition(0);

            var f = obj.GetComponent<Food>();
            f.FoodRemaining = rnd.Next(1, 10) * 100;

            food.Add(obj);
        }
    }

    private void CloneMob(GameObject mobPrefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var clone = (GameObject)Instantiate(mobPrefab);
            //clone.transform.position = RandomPosition(1f);

            var m = clone.GetComponent<Reproduction>();
            if (m != null)
            {
                m.Species = new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
                clone.renderer.material.color = m.Species;
                m.Father = m;
                m.Pregnant = true;
            }

            mobs.Add(clone);
        }
    }

    public static Vector3 RandomPosition(float yOffset = 0f)
    {
        var newPos = new Vector3(rnd.Next(-Area, Area), 0, rnd.Next(-Area, Area));
        newPos.y = Terrain.activeTerrain.SampleHeight(newPos) + yOffset;
        return newPos;
    }
}

