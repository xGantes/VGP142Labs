using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(CharacterController))]
public class PlayerControls : MonoBehaviour
{
    private Animator animator;
    private CharacterController charController;
    Vector3 moveDirection;

    [Header("Player Settings")]
    [Space(2)]
    [Tooltip("Speed Value from 1 to 8")]
    [Range(1.0f, 8.0f)]
    public float speed;
    public float rotationSpeed;
    public float jumpSpeed;
    public float gravity;

    void Awake()
    {
        try
        {
            animator = GetComponent<Animator>();
            charController = GetComponent<CharacterController>();
            charController.minMoveDistance = 0.0f;

            if (speed <= 6)
            {
                speed = 6.0f;
            }
            if (rotationSpeed <= 0)
            {
                rotationSpeed = 250f;
            }
            if (jumpSpeed <= 0)
            {
                jumpSpeed = 8.0f;
            }
            if (gravity <= 0)
            {
                gravity = 14.0f;
            }
            moveDirection = Vector3.zero;
        }
        catch (NullReferenceException e)
        {
            Debug.Log(e.Message);
        }
        catch (UnassignedReferenceException e)
        {
            Debug.Log(e.Message);
        }
        finally
        {
            Debug.Log("Calls Anytime");
        }
    }

    [ContextMenu("Reset Player Settings")]
    void PlayerReset()
    {
        speed = 6.0f;
        rotationSpeed = 250f;
        jumpSpeed = 8.0f;
        gravity = 14.0f;
    }

    void Update()
    {
        if (charController.isGrounded)
        {
            Movement();
        }
        else
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            moveDirection.x = horizontal * speed;
            moveDirection.z = vertical * speed;
        }

        moveDirection.y -= gravity * Time.deltaTime;
        charController.Move(moveDirection * Time.deltaTime);

        if (Input.GetButtonDown("Fire1"))
        {
            Fire();
        }
    }

    void Movement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveDirection = new Vector3(horizontal, 0, vertical);
        moveDirection *= speed;
        moveDirection = transform.TransformDirection(moveDirection);

        if (Input.GetButtonDown("Jump"))
        {
            moveDirection.y = jumpSpeed;
        }

        if (moveDirection != Vector3.zero)
        {
            animator.SetBool("isMoving", true);

            Quaternion charRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, charRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }
    void Fire()
    {

    }
}
