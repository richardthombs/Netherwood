using UnityEngine;

public enum Sex
{
    Asexual =0,
    Male = 1,
    Female = 2
}

public class Reproduction : MonoBehaviour
{
    public Sex Sex;
    public float GestationPeriod = 30;
    public float SeasonInterval = 120;
    public float SeasonDuration = 30;
    public float Fussiness = 0.1f;
    public int LitterSize = 1;

    public bool InSeason;
    public bool Pregnant;
    public float DueDate;
    public bool Due;

    public Color Species;

    public Reproduction Mate;
    public Reproduction Father;
    public float Speed;

    Senses senses;
    Navigator nav;
    Body body;

    void Awake()
    {
        senses = GetComponent<Senses>();
        nav = GetComponent<Navigator>();
        body = GetComponent<Body>();
        renderer.material.color = Species;
    }

    void Start()
    {
    }

    void Update()
    {
        Due = (Pregnant && DueDate > 0 && Time.time > DueDate);

        if (Pregnant && DueDate == 0) DueDate = Time.time + GestationPeriod + Random.value * 5;

        InSeason = IsMatingSeason(Time.time) && !Pregnant;
        if (!InSeason) Mate = null;

        if (InSeason && !Pregnant)
        {
            if (Sex == Sex.Asexual)
            {
                GetPregnant(this);
            }
            else
            {
                if (Mate != null)
                {
                    if (transform.position.IsCloseTo(Mate.transform.position))
                    {
                        if (Sex == Sex.Female)
                        {
                            GetPregnant(Mate);
                        }
                    }
                    else
                    {
                        nav.SetDestination(Mate.transform.position, Speed);
                    }
                }
                else
                {
                    LookForMate();
                }
            }
        }

        if (Due)
        {
            GiveBirth();
        }
    }

    void GetPregnant(Reproduction father)
    {
        Pregnant = true;
        Father = father;

        Father.Mate = null;
    }

    void LookForMate()
    {
        Mate = null;
        foreach (var m in senses.Mobiles)
        {
            var r = m.GetComponent<Reproduction>();

            if (r != null && IsSuitableMate(r))
            {
                Mate = r;
                break;
            }
        }
    }

    void GiveBirth()
    {
        Pregnant = false;
        DueDate = 0;
        Due = false;

        for (int i = 0; i < LitterSize; i++)
        {
            var baby = (GameObject)GameObject.Instantiate(gameObject, transform.position + Random.insideUnitSphere, Random.rotation);
            var babyRepro = baby.GetComponent<Reproduction>();

            if (Father == null) Father = this;
            babyRepro.Species = MutateSpecies(Species, Father.Species, Fussiness / 2);
            babyRepro.Mate = null;
            babyRepro.Father = null;
            if (Sex != Sex.Asexual) babyRepro.Sex = (Sex)Random.Range(1, 3);

            var babyBody = baby.GetComponent<Body>();
            babyBody.Stomach.Contents = body.Fat.Get(body.Fat.Contents / 2);
            babyBody.Fat.Contents = 0;

            var babyReaper = baby.GetComponent<GrimReaper>();
            babyReaper.Age = 0;

            var babyEat = baby.GetComponent<Eat>();
            babyEat.Food = null;

            var babyWalk = baby.GetComponent<RandomWalk>();
            babyWalk.Destination = Vector3.zero;
        }

        Father = null;
    }

    bool IsMatingSeason(float time)
    {
        float start = Mathf.Floor(time / SeasonInterval) * SeasonInterval;
        float end = start + SeasonDuration;

        return start <= time && time < end;
    }

    public bool IsSuitableMate(Reproduction candidate)
    {
        if (Sex == Sex.Asexual && candidate.Sex != Sex.Asexual) return false;
        if (Sex != Sex.Asexual && candidate.Sex == Sex.Asexual) return false;
        if (Sex == candidate.Sex) return false;
        if (candidate.Pregnant) return false;

        return SpeciesMatch(Species, candidate.Species, Fussiness);
    }

    public bool IsSameSpecies(Reproduction candidate)
    {
        return SpeciesMatch(Species, candidate.Species, Fussiness);
    }

    static bool SpeciesMatch(Color s1, Color s2, float tolerance)
    {
        var r = Mathf.Abs(s1.r - s2.r);
        var g = Mathf.Abs(s1.g - s2.g);
        var b = Mathf.Abs(s1.b - s2.b);

        return (r + g + b) <= tolerance;
    }

    static Color MutateSpecies(Color s1, Color s2, float mutation)
    {
        var r = Mutate(AorB(s1.r, s2.r), mutation);
        var g = Mutate(AorB(s1.g, s2.g), mutation);
        var b = Mutate(AorB(s1.b, s2.b), mutation);

        return new Color(r, g, b);
    }

    static float AorB(float a, float b)
    {
        if (Random.value >= 0.5) return b;
        return a;
    }

    static float Mutate(float v, float mutation)
    {
        var d = (Random.value - 0.5f) * mutation;
        var clamped = Mathf.Clamp(v + d, 0f, 1f);
        return clamped;
    }
}
