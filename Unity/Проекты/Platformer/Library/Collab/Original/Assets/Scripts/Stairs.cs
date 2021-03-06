﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    [SerializeField] private float speed = 10;

    void OnTriggerStay2D(Collider2D other)
    {
        other.GetComponent<Rigidbody2D>().gravityScale = 0;
        if (Input.GetKey(KeyCode.Z))
            other.GetComponent<Rigidbody2D>().velocity = new Vector2(0, speed);
        else if(Input.GetKey(KeyCode.X))
            other.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -speed);
        else
            other.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<Rigidbody2D>().gravityScale = 0;
    }
}
