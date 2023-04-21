using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float speedLimiter = 0.7f;

    float inputHorizontal;
    float inputVertical;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello");
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
        Debug.Log(inputHorizontal + ", " + inputVertical);
    }

    private void FixedUpdate()
    {
        //rb.MovePosition(rb.position + new Vector2(inputHorizontal, inputVertical) * walkSpeed * Time.fixedDeltaTime);
        if (inputHorizontal != 0 || inputVertical != 0)
        {
            rb.velocity = new Vector2 (inputHorizontal * walkSpeed, inputVertical * walkSpeed);
        }
    }
}
