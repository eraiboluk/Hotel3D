using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    string previousLayerName = "";
    public CinemachineVirtualCamera cameraMiddle;
    public CinemachineVirtualCamera cameraLeft;
    public CinemachineVirtualCamera cameraRight;

    void Update()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            string layerName = LayerMask.LayerToName(hit.collider.gameObject.layer);

            if (layerName != previousLayerName)
            {

                switch (layerName)
                {
                    case "FloorMain":
                        cameraMiddle.enabled=true;
                        break;
                    case "FloorRight":
                        cameraLeft.enabled=true;
                        cameraMiddle.enabled=false;
                        break;
                    case "FloorLeft":
                        cameraRight.enabled=true;
                        cameraLeft.enabled=false;
                        cameraMiddle.enabled=false;
                        break;
                    default:
                        cameraMiddle.enabled=true;
                        break;
                }
                previousLayerName = layerName;
                Debug.Log("Player is on the " + layerName + " layer.");
            }
        }
        else    
            Debug.Log("no hit");
    }
}
