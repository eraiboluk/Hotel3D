using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProgressBarGameplay : MonoBehaviour
{
    public GameObject targetObject;
    public float scaleFactor = 100f;
    public bool done = false;
    private Vector3 originalScale;
    public float rebornTime = 1f;

    void Awake()
    {
        if (targetObject != null)
        {
            originalScale = targetObject.transform.localScale;
        }
        else
        {
            Debug.LogError("Target GameObject is not assigned!");
        }
        gameObject.SetActive(false);
        done = true;
    }

    void Update()
    {
        if (targetObject == null)
        {
            return;
        }

        if (scaleFactor == 0)
        {
            gameObject.SetActive(false);
            done = true;
        }
        else
        {
            float targetScaleX = originalScale.x * (scaleFactor / 100f);
            Vector3 newScale = new Vector3(targetScaleX, originalScale.y, originalScale.z);
            
            targetObject.transform.localScale = newScale;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Cleaner" || other.tag == "Receptionist")
        {
            if(scaleFactor > 0)
                scaleFactor--;
        }
    }
    public void StartReborn(){ 
        gameObject.SetActive(true);
        scaleFactor=100f;
        done=false;    
    }

}

