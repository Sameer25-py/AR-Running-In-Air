﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour

{   
    private CharacterController controller;
    private Vector3 direction;
    public float forwardSpeed;
    private int desiredLane = 1;
    public float laneDistance = 4;//distance between two lanes
    public float jumpForce;
    public float Gravity = -20;
    public Animator animator;
    public bool isGrounded;
    //public LayerMask groundLayer;
    //public Transform groundCheck;
    public float maxSpeed;
    private bool isSliding=false;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!PlayerManager.isGameStarted)
            return;
        if(forwardSpeed < maxSpeed)
        {
            forwardSpeed += 0.1f*Time.deltaTime;
        }
        animator.SetBool("isGameStarted",true);
        direction.z = forwardSpeed;
       // isGrounded = Physics.CheckSphere(groundCheck.position,0.15f,groundLayer);
        animator.SetBool("isGrounded",controller.isGrounded);
        if (controller.isGrounded)
        {
            //direction.y = -1;
            if (SwipeManager.swipeUp)
            {
                Jump();
            }
        }
        else
        {
            direction.y += Gravity * Time.deltaTime;
        }
        if(SwipeManager.swipeDown && !isSliding)
        {
            StartCoroutine(Slide());
        }
        //inputs
        if (SwipeManager.swipeRight)
        {
            desiredLane++;
            if (desiredLane==3)
                desiredLane = 2;
        }
        if (SwipeManager.swipeLeft)
        {
            desiredLane--;
            if (desiredLane==-1)
                desiredLane = 0;
        }

        //calculate where we should be 
        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
        if (desiredLane==0)
        {
            targetPosition += Vector3.left * laneDistance;
        }
        else if(desiredLane == 2)
        {
            targetPosition += Vector3.right * laneDistance;
        }
        //transform.position = Vector3.Lerp(transform.position,targetPosition,800*Time.deltaTime);
        //controller.center = controller.center;
        if (transform.position == targetPosition)
        {
            return;
        }
        Vector3 diff = targetPosition - transform.position;
        Vector3 moveDir = diff.normalized * 25 * Time.deltaTime;
        if(moveDir.sqrMagnitude< diff.sqrMagnitude)
            controller.Move(moveDir);
        else
            controller.Move(diff);
    }
    void FixedUpdate()
    {
        if(!PlayerManager.isGameStarted)
            return;
        controller.Move(direction*Time.fixedDeltaTime);
    }
    private void Jump()
    {
        direction.y = jumpForce;
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag=="Obstacle")
        {
            PlayerManager.gameOver = true;
        }
    }
    private IEnumerator Slide()
    {
        isSliding = true;
        animator.SetBool("isSliding",true);
        controller.center = new Vector3(0,-0.3f,0);
        controller.height = 0.5f;
        yield return new WaitForSeconds(1.3f);
        controller.center = new Vector3(0,0,0);
        controller.height = 2;
        animator.SetBool("isSliding",false);
        isSliding = false;
    }
}
