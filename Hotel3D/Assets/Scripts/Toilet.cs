using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toilet : MonoBehaviour
{
    public bool IsOccupied = false;
    public GameObject sign;
    public Vector3 location;
    public GameObject toiletRefillArea;

    public int UseTime = 3;
    public int UseCount = 0;

    public bool isClean = true;

    void Start()
    {
        toiletRefillArea.SetActive(false);
        sign.SetActive(false);
        location = sign.transform.position;
        location.y = 0.5f;
    }

    public void Occupie(){
        IsOccupied = true;
    }

    public void UnOccupie(){
        UseCount++;
        if(UseCount >= UseTime){
            sign.SetActive(true);
            toiletRefillArea.SetActive(true);
            isClean = false;
        }
        else
            IsOccupied = false;
    }

    public void Refill(){
        isClean = true;
        UseCount = 0;
        sign.SetActive(false);
        IsOccupied = false;
        toiletRefillArea.SetActive(false);
    }
}
