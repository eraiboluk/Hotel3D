using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ServantNPC : MonoBehaviour
{
    public string roomTag = "Toilet"; 
    public string ToiletAreaTag = "ToiletRollArea"; 
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

        StartCoroutine(SearchForUnCleanRestRoomPeriodically());
    }

    IEnumerator SearchForUnCleanRestRoomPeriodically()
    {
        while (true)
        {
            if (!isSearching && items.howManyItems > 0)
            {
                isSearching = true;
                SearchForUnCleanRestRoom();
            }
            else if(items.howManyItems <= 0){
                isSearching = true;
                StartCoroutine(GetToiletRolls());
            }
            yield return new WaitForSeconds(scanInterval);
        }
    }

    void SearchForUnCleanRestRoom()
    {
        GameObject[] toilets = GameObject.FindGameObjectsWithTag(roomTag);
        float shortestDistance = Mathf.Infinity;
        Toilet nearestToilet = null;

        foreach (GameObject toilet in toilets)
        {
            Toilet toiletScript = toilet.GetComponent<Toilet>();
            
            if (toiletScript != null && !toiletScript.isClean)
            {
                float distanceToRoom = Vector3.Distance(agent.transform.position, toiletScript.toiletRefillArea.transform.position);
                if (distanceToRoom < shortestDistance)
                {
                    shortestDistance = distanceToRoom;
                    nearestToilet = toiletScript;
                }
            }
        }

        if (nearestToilet != null)
        {
            StartCoroutine(CleanRooms(nearestToilet));
        }
        else
        {
            agent.SetDestination((startingPos));
            isSearching = false;
        }
    }

    IEnumerator CleanRooms(Toilet nearestToilet)
    {
        agent.SetDestination((nearestToilet.toiletRefillArea.transform.position));
        yield return new WaitUntil(() => nearestToilet.isClean);
        isSearching = false;
    }

    IEnumerator GetToiletRolls(){
        GameObject toiletRollArea = GameObject.FindGameObjectWithTag(ToiletAreaTag);
        Debug.Log(toiletRollArea.transform.position);
        agent.SetDestination((toiletRollArea.transform.position));
        yield return new WaitUntil(() => items.howManyItems >= items.positions.Length);
        isSearching = false;
    }
}
