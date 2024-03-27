using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestRoom : MonoBehaviour
{
    public Toilet[] toilets;
    public float resetDelay = 5f; 
    public GameObject moneySpot;

    public Toilet FindEmptyToilet()
    {
        foreach (var toilet in toilets)
        {
            if (!toilet.IsOccupied)
            {
                return toilet; 
            }
        }
        return null; 
    }    
}
