using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    // Constants regarding player movement
    public float                    speed              = 12f;
    public float                    gravity            = -9.81f;
    public float                    jumpHeight         = 3f;

    // Used in determining whether the player is on the ground
    private Transform               groundCheck;
    private CharacterController     characterController;
    public float                    groundDistance     = 0.6f;
    public LayerMask                groundMask;

    Vector3                         move;
    [SerializeField] Vector3        velocity;
    public bool                     isGrounded;
    
    public float                    knockBackMultiplier = .5f;
    public float                    knockBackTime = .25f;
    public float                    knockBackCounter;
    public Vector3                  knockBackMove;

    void Start()
    {
        groundCheck = GameObject.Find("GroundCheck").GetComponent<Transform>();
        characterController = GetComponent<CharacterController>();
    }


    // Update is called once per frame
    void Update()
    {
        checkIfGrounded();

        horizontalMovement();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jump();
        }

        applyGravity();

        controller.Move(velocity * Time.deltaTime);

        if (knockBackCounter > 0)
        {
            knockBackCounter -= Time.deltaTime;

            controller.Move(knockBackMove * knockBackMultiplier * knockBackCounter * Time.deltaTime);
        }
    }

    private void checkIfGrounded()
    {
        // Set to true if the groundCheck object collides with something in the groundMask
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Removes the jittering when jumping onto objects
        if (velocity.y <= 0 && isGrounded && characterController.stepOffset != 0.3f)
        {
            // Debug.Log("Reset stepOffset");
            characterController.stepOffset = 0.3f;
        }

        if (!isGrounded && characterController.stepOffset != 0f)
        {
            // Debug.Log("Set stepOffset");
            characterController.stepOffset = 0f;
        }
    }

    private void horizontalMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Applies horizontal velocity to the player
        move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);
    }

    private void jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }


    private void applyGravity()
    {
        if (isGrounded && velocity.y < 0) // Keeps player from constantly accelerating while on the ground
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }


    public void knockback(Vector3 direction, float pushForce)
    {
        knockBackCounter = knockBackTime;

        //direction = new Vector3(1f, 1f, 1f);
        knockBackMove = direction * pushForce;
    }
}
