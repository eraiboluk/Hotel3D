using UnityEngine;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AI;
using System.Collections.Generic;

public class Rooms : MonoBehaviour
{
    public int roomLevel;
    public string[] roomAddresses;
    public string[] roomAddressesUpgrade;
    public Vector3[] roomPositions;
    public GameObject[] walls;
    public GameObject[] buyAreas;
    private Queue<GameObject> handles;
    public int currentRoomIndex = 0;

    private NavMeshSurface navMeshSurface;

    public bool Checking = false;

    public float waitNavMesh = 0.5f;

    public string dataSaveTag = "Data";
    DataBaseSaver loader = null;

    void Start()
    {
        GameObject gameobject = GameObject.FindGameObjectWithTag(dataSaveTag);
        loader =gameobject.GetComponent<DataBaseSaver>();
        handles = new Queue<GameObject>(roomAddresses.Length);
        GameObject navmesh = GameObject.FindGameObjectWithTag("NavMesh");
        navMeshSurface = navmesh.GetComponent<NavMeshSurface>();
        StartCoroutine(SetInitial());
    }

    public void Purchased()
    {
        if(!Checking)
            StartCoroutine(CheckAndActivateRoom());
    }

    IEnumerator CheckAndActivateRoom()
    {
        Checking = true;

        if (currentRoomIndex < roomAddresses.Length)
        {
            yield return LoadAndActivateRoom(currentRoomIndex);
            ActivateNextBuyArea();
        }
        else if(currentRoomIndex < buyAreas.Length)
        {
            yield return LoadAndUpgradeRoom(currentRoomIndex-roomAddresses.Length);
            ActivateNextBuyArea();
        }
        yield return new WaitForSeconds(waitNavMesh);
        navMeshSurface.BuildNavMesh();
        Checking = false;
    }

    IEnumerator LoadAndActivateRoom(int roomIndex)
    {
        string roomAddress = roomAddresses[roomIndex];

        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(roomAddress);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject roomPrefab = handle.Result;
            
            GameObject room = Instantiate(roomPrefab, roomPositions[roomIndex], Quaternion.identity);

            handles.Enqueue(room);
            Destroy(walls[roomIndex]);
        }
        else
        {
            Debug.LogError($"Failed to load room prefab: {roomAddress}");
        }
    }

    IEnumerator LoadAndUpgradeRoom(int roomIndex)
    {
        string roomAddress = roomAddressesUpgrade[roomIndex];
        
        GameObject roomDestroy = handles.Dequeue();
        Destroy(roomDestroy);

        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(roomAddress);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject roomPrefab = handle.Result;
            
            GameObject room = Instantiate(roomPrefab, roomPositions[roomIndex], Quaternion.identity);
        }
        else
        {
            Debug.LogError($"Failed to load room prefab: {roomAddress}");
        }
    }

    void ActivateNextBuyArea()
    {
        currentRoomIndex++;
        loader.dts.crrLevels[roomLevel] = currentRoomIndex;
        loader.SaveDataFn();
        if (currentRoomIndex < buyAreas.Length)
        {
            buyAreas[currentRoomIndex].SetActive(true);
        }

    }
    IEnumerator SetInitial(){
        yield return new WaitUntil(() => loader.loaded == true);
        currentRoomIndex = loader.dts.crrLevels[roomLevel];
        yield return SetInitialState();
        Time.timeScale = 1;
        yield return new WaitForSeconds(waitNavMesh);
        navMeshSurface.BuildNavMesh();
    }
    IEnumerator SetInitialState()
    {
        foreach (GameObject wall in walls)
        {
            if(wall != null)
                wall.SetActive(true);
        }

        foreach (GameObject buyArea in buyAreas)
        {
            buyArea.SetActive(false);
        }

        for(int i=0; i < currentRoomIndex; i++)
        {
            if (i < roomAddresses.Length)
            {
                yield return LoadAndActivateRoom(i);
                if((i+1)<=buyAreas.Length-1){
                    buyAreas[i+1].SetActive(true);
                    buyAreas[i].SetActive(false);
                }
                else if((i)<=buyAreas.Length-1)
                    buyAreas[i].SetActive(false);

                Destroy(walls[i]);
            }
            else if(i < buyAreas.Length)
            {
                yield return LoadAndUpgradeRoom(i-roomAddresses.Length);
                if((i+1)<=buyAreas.Length-1){
                    buyAreas[i+1].SetActive(true);
                    buyAreas[i].SetActive(false);
                }
                else if((i)<=buyAreas.Length-1)
                    buyAreas[i].SetActive(false);
            }   
            navMeshSurface.BuildNavMesh();
        }
        
        if (currentRoomIndex < buyAreas.Length)
        {
            buyAreas[currentRoomIndex].SetActive(true);
        }
    }
}
