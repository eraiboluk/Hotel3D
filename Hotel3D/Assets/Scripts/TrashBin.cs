using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public float setTime = 1f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")){

            setTime -= Time.deltaTime;

            if(setTime <= 0){
                FindObjectOfType<AudioManager>().Toggle("Trash",1);
                other.GetComponent<PlayerHands>().removeItem();
                setTime = 1f;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        setTime = 1f;
    }
}
