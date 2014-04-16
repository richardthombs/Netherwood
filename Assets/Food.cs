using UnityEngine;
using System.Collections;

public class Food : MonoBehaviour
{
    public float FoodRemaining;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public float Eat(float qty)
    {
        var eaten=FoodRemaining <= qty? FoodRemaining : qty;
        FoodRemaining-=eaten;
        if (FoodRemaining <= 0) Destroy(gameObject, 1);
        return eaten;
    }
}
