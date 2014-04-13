using UnityEngine;
using System.Collections.Generic;

public class MainObject : MonoBehaviour
{
    static System.Random rnd = new System.Random();
    List<GameObject> mobs = new List<GameObject>();
    List<GameObject> food = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        var mobPrefab = GameObject.Find("mobPrefab");
        CloneMob(mobPrefab, 5000);

        var foodPrefab = GameObject.Find("foodPrefab");
        for (int i = 0; i < 1000; i++)
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
            m.Health = rnd.Next(1, 10) * 10;
            m.Energy = rnd.Next(1, 10) * 10;
            m.Destination = RandomPosition(1f);

            mobs.Add(obj);
        }
    }

    public static Vector3 RandomPosition(float yOffset = 0f)
    {
        int area = 1000;
        var newPos = new Vector3(rnd.Next(-area, area), 0, rnd.Next(-area, area));
        newPos.y = Terrain.activeTerrain.SampleHeight(newPos) + yOffset;
        return newPos;
    }
}

