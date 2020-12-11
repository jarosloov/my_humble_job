using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CharacterControllerScript : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    private float moveInput;
    private Rigidbody2D rb;
    public static Vector3 Spawnpoint = new Vector3(0f, 0f, 0f);
    public Vector3 DefaultSpawnpoint = new Vector3(-40f, -113.6f, -15f);
    private bool facingRight = true;
    private void Start()
    {
        if (Spawnpoint == Vector3.zero)
        {
            transform.position = DefaultSpawnpoint;
            Debug.Log(transform.position);
        }
        else
        {
            transform.position = Spawnpoint;
            Debug.Log(transform.position);
        }

        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

    }
}