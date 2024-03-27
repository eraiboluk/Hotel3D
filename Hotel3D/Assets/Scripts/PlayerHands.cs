using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHands : MonoBehaviour
{
    public int howManyItems = 0;
    public Stack<GameObject> myStack;
    public GameObject[] positions;
    public string itemType;

    private Animator animator = null;

    void Start(){
        animator = GetComponent<Animator>();
        myStack = new Stack<GameObject>();
    }

    public void putItem(GameObject item){
        if(howManyItems == 0){
            itemType = item.name;
            if(animator != null)
                animator.SetBool("IsCarrying", true);
        }
        if(item.name == itemType && myStack.Count < positions.Length){
            GameObject cloneItem = Instantiate(item, positions[howManyItems].transform.position, transform.rotation, transform);
            myStack.Push(cloneItem);
            howManyItems++;
        } 
    }
    public void removeItem(){
        if(howManyItems > 0){
            Destroy(myStack.Pop());
            howManyItems--;
            if(howManyItems == 0)
                if(animator != null)
                    animator.SetBool("IsCarrying", false);
        }
    }
}
