using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerXY : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    
    

    
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        
        MyInput();
        SpeedControl();

        //handling drag
       
            rb.drag = groundDrag *(1.2f);
        
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    // Update is called once per frame
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
       

       
    }

    private void MovePlayer()
    {
         moveDirection = orientation.transform.forward * verticalInput + orientation.transform.right * horizontalInput;

         
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
         
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //limit velocity
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
       
    }
   
    
}
