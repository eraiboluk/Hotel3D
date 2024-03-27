using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class NPCBehavior : MonoBehaviour
{
    public string roomTag = "Room"; 
    public string RestRoomTag = "RestRoom";
    public string RoomServiceTag = "RoomService";
    public float scanInterval = 1f; 
    public float sleepTime = 3f;
    private NavMeshAgent agent;
    private bool isSearching = false;
    public Vector3 referencePoint;
    private Animator animator;
    public Vector3 NPCDestroy;
    public bool foundRoom = false;
    public bool isServiced = false;

    public GameObject moneyGenerator;
    private MoneyGeneration money;

    public Vector3 ReceptionMoneyLocation;
    public int BookingPrice;

    public int RoomTipValue;

    public GameObject sleepSign;
    public GameObject peeSign;
    public GameObject roomServiceSign;
    public GameObject roomServiceArea;

    void Awake()
    {
        money= moneyGenerator.GetComponent<MoneyGeneration>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        sleepSign.SetActive(false);
        peeSign.SetActive(false);
        roomServiceSign.SetActive(false);
        roomServiceArea.SetActive(false);
    }

    public void GoGo(Vector3 dest){ StartCoroutine(GoThere(dest)); }

    IEnumerator GoThere(Vector3 dest){

        agent.SetDestination(dest);
        animator.SetBool("isWalking", true);
        yield return new WaitUntil(() => Vector3.Distance(agent.transform.position, dest) < 1f);
        animator.SetBool("isWalking", false);
        
    }

    public void YourTurn(){
        StartCoroutine(SearchForCleanRoomPeriodically());
    }

    IEnumerator SearchForCleanRoomPeriodically()
    {
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
        GameObject nearestRoom = null;

        foreach (GameObject room in rooms)
        {
            Sanitation sanitation = room.GetComponent<Sanitation>();

            if (sanitation != null && sanitation.isClean && !sanitation.occupied)
            {
                float distanceToRoom = Vector3.Distance(transform.position, (room.transform.position));
                if (distanceToRoom < shortestDistance)
                {
                    shortestDistance = distanceToRoom;
                    nearestRoom = room;
                }
            }
        }

        if (nearestRoom != null)
        {
            FindObjectOfType<AudioManager>().Toggle("Key",1);
            StartCoroutine(WaitAtDestination((nearestRoom.transform.position), nearestRoom));

        }
        else
        {
            isSearching = false;
        }
    }

    IEnumerator WaitAtDestination(Vector3 destination, GameObject nearestRoom)
    {
        yield return new WaitForSeconds(1f);
        Debug.Log(agent + "  " + (nearestRoom.transform.position));
        agent.SetDestination((nearestRoom.transform.position));
        animator.SetBool("isWalking", true);
        foundRoom = true;

        Sanitation room = nearestRoom.GetComponent<Sanitation>();
        room.occupied=true;

        money.SetMoney(BookingPrice, ReceptionMoneyLocation);

        yield return new WaitUntil(() => Vector3.Distance(agent.transform.position, destination) < 1f);
        animator.SetBool("isWalking", false);
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        sleepSign.SetActive(true);
        yield return new WaitForSeconds(sleepTime);
        sleepSign.SetActive(false);
        Vector3 moneyPosition = agent.transform.position;

        int DoWhat = Random.Range(0, 3);
        Debug.Log(DoWhat);
        //Go To Toilet
        if(DoWhat == 0)
        {
            GameObject[] RestRooms = GameObject.FindGameObjectsWithTag(RestRoomTag);
        
            if(RestRooms.Length > 0)
            {
                Toilet toilet = null;
                peeSign.SetActive(true);
                animator.SetBool("isPeing", true);

                yield return new WaitUntil(() => {
                    toilet = SearchForRestRoom(RestRooms); 
                    return toilet != null;
                });

                Debug.Log(toilet.location);
                yield return new WaitUntil(() => Vector3.Distance(agent.transform.position, toilet.location) < 1f);
                animator.SetBool("isPeing", false);
                toilet.UnOccupie(); 

                moneyPosition = agent.transform.position;
                moneyPosition.z -= 2f;
                
                peeSign.SetActive(false);
            }
        }
        //Request Room Service
        else if(DoWhat == 1){

            GameObject[] RoomServices = GameObject.FindGameObjectsWithTag(RoomServiceTag);

            if(RoomServices.Length > 0)
            {
                roomServiceSign.SetActive(true);
                roomServiceArea.SetActive(true);
                animator.SetBool("isRaising", true);
                yield return new WaitUntil(() => isServiced);
                animator.SetBool("isRaising", false);
                roomServiceSign.SetActive(false);
                roomServiceArea.SetActive(false);
            }
        }
        if(room != null){
            room.customerLeft();

            yield return new WaitForSeconds(room.checkInterval); //beklememiz gerekiyor çümkü temiz olmadığı algılanmadan occipiedlıktan çıkarsa bazen başka NPC temiz olmayan odaya giriyor.
            room.occupied=false;
        }
        agent.SetDestination(NPCDestroy);

        money.SetMoney(RoomTipValue, moneyPosition);

        animator.SetBool("isWalking", true);
        yield return new WaitUntil(() => Vector3.Distance(agent.transform.position, NPCDestroy) < 1f);
        Destroy(gameObject);
    }

    private Toilet SearchForRestRoom(GameObject[] RestRooms)
    {
        float shortestDistance = Mathf.Infinity;
        Vector3 nearestPosition = Vector3.zero;

        foreach (GameObject RestRoom in RestRooms)
        {
            RestRoom room = RestRoom.GetComponent<RestRoom>();
            Toilet toilet = room.FindEmptyToilet();

            if (toilet != null)
            {
                float distanceToRoom = Vector3.Distance(transform.position, (room.transform.position));
                if (distanceToRoom < shortestDistance)
                {
                    shortestDistance = distanceToRoom;
                    nearestPosition = toilet.location;
                }
            }

            if (nearestPosition != Vector3.zero)
            {
                agent.SetDestination(nearestPosition);
                toilet.Occupie();
                return toilet;
            }
        }
        return null;
    }

    public void RingTheBell(){
        FindObjectOfType<AudioManager>().Toggle("Bell",1);
    }
}
