using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomServiceItem : MonoBehaviour
{
    public GameObject Item;

    void Start(){
        Item.name = "RoomService";
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("RoomServiceNPC")){
            if(other.GetComponent<PlayerHands>().myStack.Count <= 0){
                other.GetComponent<PlayerHands>().putItem(Item);
            }
        }
    }
}