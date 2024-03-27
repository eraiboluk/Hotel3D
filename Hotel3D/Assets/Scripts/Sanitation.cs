using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sanitation : MonoBehaviour
{
    public GameObject[] targetObjects; 
    private bool checking = false;
    public bool isClean = true;
    public bool occupied = false;
    public float checkInterval = 1f;
    public int i = 0;

    void Update()
    {
        if(!checking)
            StartCoroutine(CheckCleaning());
    }
    IEnumerator CheckCleaning()
    {
        checking = true;
        int cleanCount = 0;
        for(int i=0; i<targetObjects.Length; i++){
            ProgressBarGameplay bar = targetObjects[i].GetComponent<ProgressBarGameplay>();
            if(bar.done)
                cleanCount++;
        }
        if(cleanCount >= targetObjects.Length)
            isClean=true;
        else{
            isClean=false;
        }
        yield return new WaitForSeconds(checkInterval);
        checking = false;
    }
    public void customerLeft(){
        foreach (GameObject targetObject in targetObjects)
        {
            ProgressBarGameplay bar = targetObject.GetComponent<ProgressBarGameplay>();
            bar.StartReborn();
        }
    }
}
