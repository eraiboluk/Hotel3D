using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToiletRefillArea : MonoBehaviour
{
    public Toilet toilet;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Servant"))
        {
            PlayerHands player = other.GetComponent<PlayerHands>();
            if(player.itemType == "ToiletPaper" && player.howManyItems > 0){
                if (other.CompareTag("Player"))
                    FindObjectOfType<AudioManager>().Toggle("PickUp",1);
                player.removeItem();
                toilet.Refill();
            }
        }
    }
}
