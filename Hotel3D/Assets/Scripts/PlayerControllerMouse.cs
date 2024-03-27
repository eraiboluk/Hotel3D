using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerMouse : MonoBehaviour
{
    public float moveSpeed;
    private Vector3 startPos;
    public float inputThreshold;

    public bool firstTouch = true;

    private Animator animator;
    private CharacterController controller;
    
    public float rotationAngle = 25f;

    void Start(){
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>(); 
    }
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if(firstTouch){
                startPos = Input.mousePosition;
                firstTouch=false;
            }

            Vector3 curdata = Input.mousePosition;
            float touchDistance = Vector2.Distance(startPos, curdata);

            if (touchDistance > inputThreshold)
            {
                animator.SetBool("isWalking", true);
                Vector3 moveDirection = (curdata - startPos).normalized;

                Quaternion rotation;

                switch (whichLayer())
                {
                    case "FloorMain":
                        break;
                    case "FloorRight":
                        rotation = Quaternion.AngleAxis(-rotationAngle, transform.position);
                        moveDirection = rotation * moveDirection;
                        break;
                    case "FloorLeft":
                        rotation = Quaternion.AngleAxis(rotationAngle, transform.position);
                        moveDirection = rotation * moveDirection;
                        break;
                    default:
                        break;
                }

                moveDirection.z=moveDirection.y;// gelen input vector2(x,y) tabanında hareket ediyor bunu oyunda x,z tabanına ceviriyoruz çünkü karakterin y tabaında hareketi olmamalı
                moveDirection.y = 0f;

                Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                controller.transform.rotation = Quaternion.RotateTowards(controller.transform.rotation, toRotation, Time.deltaTime * 500f); // Character Controller'ın rotasyonunu değiştirme
                                
                Vector3 newPosition = moveDirection * moveSpeed * Time.deltaTime;

                controller.Move(newPosition);
            }
            else
                animator.SetBool("isWalking", false);
        }
        else{
            firstTouch=true;
            animator.SetBool("isWalking", false);
        }
    }
    private string whichLayer(){

        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            return LayerMask.LayerToName(hit.collider.gameObject.layer);

        return "NoHit";
    }
}
