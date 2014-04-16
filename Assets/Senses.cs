using UnityEngine;
using System.Collections.Generic;

public class Senses : MonoBehaviour
{
    public List<Food> Food = new List<Food>();
    public List<GameObject> Mobiles = new List<GameObject>();

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
        Food = new List<Food>();
        Mobiles = new List<GameObject>();

        var colliders = Physics.OverlapSphere(transform.position, ViewRange);

        foreach (var col in colliders)
        {
            if (col.gameObject == gameObject) continue;

            var f = col.GetComponent<Food>();
            if (f != null) Food.Add(f);

            var m = col.GetComponent<Navigator>();
            if (m != null) Mobiles.Add(m.gameObject);
        }

        Food.Sort((a, b) => { float da = Vector3.Distance(a.transform.position, transform.position), db = Vector3.Distance(b.transform.position, transform.position); return da.CompareTo(db); });
    }
}
