﻿using UnityEngine;
using System.Collections.Generic;

public class Herd : MonoBehaviour
{
    public float TooFar = 10;
    public float TooClose = 4;
    public float Speed = 7;
    public GameObject Target;

    Navigator nav;
    Senses senses;

    void Awake()
    {
        nav = GetComponent<Navigator>();
        senses = GetComponent<Senses>();
    }

    void Update()
    {
        if (Target != null)
        {
            if (Target.transform.position.DistanceTo(transform.position) < TooClose)
            {
                Target = null;
            }
            else
            {
                nav.SetDestination(Target.transform.position, Speed);
            }
        }
        else
        {
            var herd = GetVisibleHerd();
            if (herd.Count == 0) return;

            if (!herd.Exists(x => x.transform.position.DistanceTo(transform.position) < 10))
            {
                herd.Sort((a, b) => { float da = a.transform.position.DistanceTo(transform.position), db = b.transform.position.DistanceTo(transform.position); return da.CompareTo(db); });
                Target = herd[0];
            }
        }
    }

    List<GameObject> GetVisibleHerd()
    {
        // Get a list of other herd members we can see
        var me = GetComponent<Reproduction>();
        List<GameObject> nearby = senses.Mobiles.FindAll(go =>
        {
            var r = go.GetComponent<Reproduction>();
            return r != null && me != null && me.IsSameSpecies(r);
        });
        return nearby;
    }
}

public class Herdx : MonoBehaviour
{
    public Vector3 LastKnownPosition;
    public float Speed = 7;
    public bool Lonely;
    public float LastSeen;
    public float LonlinessThreshold;
    public float WaitFor = 20;
    public float WaitedFor;
    public float Proximity = 10;

    Navigator nav;
    Senses senses;

    // Use this for initialization
    void Start()
    {

    }

    void Awake()
    {
        nav = GetComponent<Navigator>();
        senses = GetComponent<Senses>();
    }

    // Update is called once per frame
    void Update()
    {
        Lonely = LastSeen > LonlinessThreshold;

        if (Lonely)
        {
            // Move to where we last saw someone
            if (LastKnownPosition != Vector3.zero)
            {
                if (!transform.position.IsCloseTo(LastKnownPosition))
                {
                    nav.SetDestination(LastKnownPosition, Speed);
                }
                else
                {
                    nav.StayStill();

                    WaitedFor += Time.deltaTime;
                    if (WaitedFor > WaitFor)
                    {
                        WaitedFor = 0;
                        LastKnownPosition = Vector3.zero;
                    }
                }
            }

            // We will fall through to RandomWalk
        }

        // Get a list of other herd members we can see
        var me = GetComponent<Reproduction>();
        List<GameObject> nearby = senses.Mobiles.FindAll(go =>
        {
            var r = go.GetComponent<Reproduction>();
            return r != null && me != null && me.IsSameSpecies(r);
        });

        if (nearby.Count > 0)
        {
            // Record where we last saw someone
            float x = 0, y = 0, z = 0;
            foreach (var obj in nearby)
            {
                x += obj.transform.position.x;
                y += obj.transform.position.y;
                z += obj.transform.position.z;
            }
            LastKnownPosition = new Vector3(x / nearby.Count, y / nearby.Count, z / nearby.Count);
        }

        // If there is somebody nearby, we're not lonely
        if (nearby.Exists(n => Vector3.Distance(n.transform.position, transform.position) < Proximity))
        {
            LastSeen = 0;
        }
        else
        {
            LastSeen += Time.deltaTime;
        }
    }
}
