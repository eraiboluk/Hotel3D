using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CleanerNPC : MonoBehaviour
{
    public string roomTag = "Room"; 
    public float scanInterval = 1f; 
    public float sleepTime = 3f;
    private NavMeshAgent agent;
    private bool isSearching = false;
    public Vector3 referencePoint;
    public Vector3 startingPos;

    public ObstacleAvoidanceType newAvoidanceType;
    public float newSpeed = 5f;
    public float newAcc = 100f;

    public float maxDistance = 20f;

    void Start()
    {
        gameObject.AddComponent<NavMeshAgent>();

        agent = GetComponent<NavMeshAgent>();
        
        agent.obstacleAvoidanceType = newAvoidanceType;
        agent.speed = newSpeed;
        agent.acceleration = newAcc;
        startingPos = agent.transform.position;

        StartCoroutine(SearchForUnCleanRoomPeriodically());
    }

    IEnumerator SearchForUnCleanRoomPeriodically()
    {
        //yield return StartCoroutine(AddNavMesh());

        while (true)
        {
            if (!isSearching)
            {
                isSearching = true;
                SearchForCleanRoom();
            }
            yield return new WaitForSeconds(scanInterval);
        }
    }

    void SearchForCleanRoom()
    {
        GameObject[] rooms = GameObject.FindGameObjectsWithTag(roomTag);
        float shortestDistance = Mathf.Infinity;
        Sanitation nearestRoom = null;

        foreach (GameObject room in rooms)
        {
            Sanitation sanitation = room.GetComponent<Sanitation>();
            float distance = Vector3.Distance(startingPos, (room.transform.position));
            
            if (sanitation != null && !sanitation.isClean && !sanitation.occupied && distance < maxDistance)
            {
                float distanceToRoom = Vector3.Distance(agent.transform.position, (room.transform.position));
                if (distanceToRoom < shortestDistance)
                {
                    shortestDistance = distanceToRoom;
                    nearestRoom = sanitation;
                }
            }
        }

        if (nearestRoom != null)
        {
            StartCoroutine(CleanRooms(nearestRoom));
        }
        else
        {
            agent.SetDestination((startingPos));
            isSearching = false;
        }
    }

    IEnumerator CleanRooms(Sanitation nearestRoom)
    {
        foreach (GameObject clean in nearestRoom.targetObjects){
            if(nearestRoom != null){
                ProgressBarGameplay bar = clean.GetComponent<ProgressBarGameplay>();
                if(!bar.done)
                {   
                    agent.SetDestination((clean.transform.position));
                    yield return new WaitUntil(() => bar == null || bar.done);
                }
            }
        }
        isSearching = false;
    }
/*
    IEnumerator AddNavMesh()
    {
        //NavMeshHit closestHit;
        //yield return new WaitUntil(() => !NavMesh.SamplePosition(transform.position, out closestHit, 500, 1));

        gameObject.AddComponent<NavMeshAgent>();

        agent = GetComponent<NavMeshAgent>();
        
        agent.obstacleAvoidanceType = newAvoidanceType;
        agent.speed = newSpeed;
        agent.acceleration = newAcc;
        startingPos = agent.transform.position;

    }*/
}
