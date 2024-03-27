using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToiletPaper : MonoBehaviour
{
    public GameObject Item;
    public float setTime = 1f;

    void Start(){
        Item.name = "ToiletPaper";
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Servant")){

            setTime -= Time.deltaTime;

            if(setTime <= 0){
                if (other.CompareTag("Player"))
                    FindObjectOfType<AudioManager>().Toggle("PickUp",1);
                other.GetComponent<PlayerHands>().putItem(Item);
                setTime = 1f;
            }
 
        }
    }
    private void OnTriggerExit(Collider other)
    {
        setTime = 1f;
    }
}
