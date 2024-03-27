using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoomServiceNPC : MonoBehaviour
{
    public string roomTag = "RoomServiceNeed"; 
    public string ToiletAreaTag = "RoomServiceRefill"; 
    public float scanInterval = 1f; 
    private NavMeshAgent agent;
    private bool isSearching = false;
    public Vector3 startingPos;

    public ObstacleAvoidanceType newAvoidanceType;
    public float newSpeed = 5f;
    public float newAcc = 100f;

    private PlayerHands items;

    void Start()
    {
        NavMeshHit closestHit;
        NavMesh.SamplePosition(transform.position, out closestHit, 500, 1);
        transform.position = closestHit.position;
        gameObject.AddComponent<NavMeshAgent>();

        agent = GetComponent<NavMeshAgent>();
        agent.obstacleAvoidanceType = newAvoidanceType;
        agent.speed = newSpeed;
        agent.acceleration = newAcc;
        startingPos = agent.transform.position;
        items = GetComponent<PlayerHands>();

        StartCoroutine(SearchForServicePeriodically());
    }

    IEnumerator SearchForServicePeriodically()
    {
        while (true)
        {
            if (!isSearching && items.howManyItems > 0)
            {
                isSearching = true;
                SearchForService();
            }
            else if(items.howManyItems <= 0){
                isSearching = true;
                StartCoroutine(GetRoomService());
            }
            yield return new WaitForSeconds(scanInterval);
        }
    }

    void SearchForService()
    {
        GameObject customer = GameObject.FindGameObjectWithTag(roomTag);
        
        if (customer != null){
            RoomServiceArea roomServiceArea = customer.GetComponent<RoomServiceArea>();
            StartCoroutine(DeliverRoomService(roomServiceArea));
        }
        else
        {
            agent.SetDestination((startingPos));
            isSearching = false;
        }
    }

    IEnumerator DeliverRoomService(RoomServiceArea roomServiceArea)
    {
        agent.SetDestination((roomServiceArea.transform.position));
        yield return new WaitUntil(() => items.howManyItems == 0);
        isSearching = false;
    }

    IEnumerator GetRoomService(){
        GameObject toiletRollArea = GameObject.FindGameObjectWithTag(ToiletAreaTag);
        Debug.Log(toiletRollArea.transform.position);
        agent.SetDestination((toiletRollArea.transform.position));
        yield return new WaitUntil(() => items.howManyItems >= 1);
        isSearching = false;
    }
}
