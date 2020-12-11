using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    [Header("Наносимый урон:")]
    public float damage = 15;

    private Transform player;
    private Vector2 target;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        target = new Vector2(player.position.x, player.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (transform.position.x == target.x && transform.position.y == target.y)
            DestroyProjectFile();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Enemy target = other.GetComponent<Enemy>();
            if (target != null) target.AdjustHP(-damage);
            DestroyProjectFile();
        }
    }
    void DestroyProjectFile()
    {
        Destroy(gameObject);
    }
}
