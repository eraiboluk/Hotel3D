using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Reception : MonoBehaviour
{
    public GameObject[] customerPrefabs;
    public Vector3 spawnPoint; 
    public Vector3[] queuePositions;
    public float spawnInterval = 2f;
    public ProgressBarGameplay progressBar;
    private bool Spawning = false;
    public bool Moving = true;

    private bool oneTime = true;

    private List<GameObject> customersQueue = new List<GameObject>();

    void Start(){
        progressBar.done = false;
    }

    IEnumerator SpawnCustomer()
    {
        Spawning = true;
        GameObject randomCustomerPrefab = customerPrefabs[Random.Range(0, customerPrefabs.Length)];

        GameObject newCustomer = Instantiate(randomCustomerPrefab, spawnPoint, Quaternion.identity);
        customersQueue.Add(newCustomer); 
        int where = customersQueue.Count-1;
        newCustomer.GetComponent<NPCBehavior>().GoGo(queuePositions[where]);

        yield return new WaitUntil(() => Vector3.Distance(newCustomer.transform.position, queuePositions[where]) < 1f);

        if(oneTime){
            progressBar.StartReborn();
            oneTime=false;
            FindObjectOfType<AudioManager>().Toggle("Bell",1);
        }
        yield return new WaitForSeconds(spawnInterval);
        Spawning = false;
    }

    void Update(){

        if (customersQueue.Count < queuePositions.Length && !Spawning)
        {
            StartCoroutine(SpawnCustomer());
        }  

        if (progressBar.done && !Moving)
        {
            StartCoroutine(MoveQueue());
        }
    }
    
    IEnumerator MoveQueue()
    {
        if (customersQueue.Count > 0)
        {
            Moving=true;
            NPCBehavior frontCustomer = customersQueue[0].GetComponent<NPCBehavior>();
            frontCustomer.YourTurn();
            yield return new WaitUntil(() => frontCustomer.foundRoom == true);
            customersQueue.RemoveAt(0);

            int i = 0;
            foreach (GameObject customer in customersQueue)
            {
                NPCBehavior thisCustomer = customer.GetComponent<NPCBehavior>();
                thisCustomer.GoGo(queuePositions[i]);
                i++;
            }

            if(customersQueue.Count != 0){
                yield return new WaitUntil(() => Vector3.Distance(customersQueue[0].transform.position, queuePositions[0]) < 1f);
                frontCustomer.RingTheBell();
                progressBar.StartReborn();
            }
            else{
                progressBar.done = false;
                oneTime = true;
            }
        }
        Moving=false;
    }
}
