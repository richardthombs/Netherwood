using UnityEngine;
using System.Collections.Generic;

public class Senses : MonoBehaviour
{
    public List<Food> Food = new List<Food>();
    public float LookInterval = 1;
    public float ViewRange = 20;
    public float nextLook;


    // Use this for initialization
    void Start()
    {
        nextLook = Random.value;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextLook)
        {
            nextLook = Time.time + LookInterval;
            Look();
        }
    }

    void Look()
    {
        var colliders = Physics.OverlapSphere(transform.position, ViewRange);
        Food = new List<Food>();

        foreach (var col in colliders)
        {
            if (col.gameObject == gameObject) continue;

            var f = col.GetComponent<Food>();
            if (f != null) Food.Add(f);
        }

       Food.ForEach(x => x.renderer.material.color = Color.red);
    }
}
