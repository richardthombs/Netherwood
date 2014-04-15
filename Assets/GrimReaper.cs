using UnityEngine;
using System.Collections;

public class GrimReaper : MonoBehaviour
{
    public float Age;
    public float Lifespan;
    public bool Dead;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Dead) return;

        Age += Time.deltaTime;
        if (Age >= Lifespan) Die();
    }

    public void Die()
    {
        Dead = true;
        gameObject.AddComponent<Rigidbody>();
        rigidbody.useGravity = true;
        Destroy(gameObject, 30f);

        var components = GetComponents<MonoBehaviour>();
        foreach (var c in components)
        {
            Destroy(c);
        }
    }
}
