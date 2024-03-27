using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomServiceArea : MonoBehaviour
{
    public NPCBehavior npc;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("RoomServiceNPC"))
        {
            PlayerHands player = other.GetComponent<PlayerHands>();
            if(player.itemType == "RoomService" && player.howManyItems > 0){
                player.removeItem();
                npc.isServiced = true;
            }
        }
    }
}
