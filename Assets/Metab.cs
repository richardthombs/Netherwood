using UnityEngine;
using System.Collections;

public class Metab : MonoBehaviour
{
    public Stomach Stomach = new Stomach();
    public FatReserve Fat = new FatReserve();
    public Metabolism Metabolism = new Metabolism();
    public bool Hungry;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float elapsed = Time.deltaTime;

        // How much energy do we need?
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
        
        if (energyRequired > 0)
        {
            // If we don't have enough energy, we die
            var death = GetComponent<Death>();
            if (death) death.Die();
        }

        // We're hungry if our stomach is less than 25% full
        Hungry = (Stomach.Contents < Stomach.Capacity * 0.25);
    }
}
