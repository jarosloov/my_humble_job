using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    public float speed;
    public float stoppingdistance;
    public float retreatDistance;
    public float searchDistance = 3;
    public float checkDistance = 1;
    public float height = 2;

    private float timeBtwShots;
    public float startTimeBtwShots;

    public GameObject projectile;
    public Transform shootpoint;
    private Transform player;
    private Rigidbody2D body;
    private int layerMask;
    SpriteRenderer sprite;

    public float time = 1.5f;

    [SerializeField] private LayerMask targetMask; // маски целей
    [SerializeField] private LayerMask ignoreMask; // маски, которые нужно игнорировать (например, маска данного персонажа)
    [SerializeField] [Range(1, 6)] private int rays = 3; // число лучей по формуле (N * 2) - 1, где N - данная переменная
    [SerializeField] [Range(1, 30)] private float distance = 5; // длинна луча
    [SerializeField] [Range(0, 90)] private float angle = 20; // угол между лучами
    [SerializeField] private Transform rayPoint; // объект, из которого выпускаются лучи
    private int invert = 1;

    // только для платформера, когда персонаж может поворачивается лицом влево/вправо
    // где по умолчанию, на старте, персонаж смотрит вправо
    // если в игре персонаж всегда следит за мышкой (вид сверху) то эту функцию использовать не нужно
    public void PlatfomerFlip()
    {
        time = 1.5f;
        transform.Rotate(0f, 180f, 0f);
    }

    bool GetRay(Vector2 dir)
    {
        bool result = false;

        RaycastHit2D hit = Physics2D.Raycast(rayPoint.position, dir, distance, ~ignoreMask);

        if (hit.collider != null)
        {
            if (CheckObject(hit.collider.gameObject))
            {
                result = true;
                Debug.DrawLine(rayPoint.position, hit.point, Color.green);
                // луч попал в цель
            }
            else
            {
                Debug.DrawLine(rayPoint.position, hit.point, Color.blue);
                // луч попал в любой другой коллайдер
            }
        }
        else
        {
            Debug.DrawRay(rayPoint.position, dir * distance, Color.red);
            // луч никуда не попал
        }
        return result;
    }

    bool CheckObject(GameObject obj)
    {
        if (((1 << obj.layer) & targetMask) != 0)
        {
            return true;
        }

        return false;
    }

    bool Scan()
    {
        bool hit = false;
        float j = 0;
        for (int i = 0; i < rays; i++)
        {
            var x = Mathf.Sin(j);
            var y = Mathf.Cos(j);

            j += angle * Mathf.Deg2Rad / rays * invert;

            if (x != 0)
            {
                if (GetRay(rayPoint.TransformDirection(new Vector3(y, -x, 0)))) hit = true;
            }

            if (GetRay(rayPoint.TransformDirection(new Vector3(y, x, 0)))) hit = true;
        }

        return hit;
    }

    bool CheckPath() // проверка поверхности на пути следования
    {
        Vector3 pos = new Vector3(transform.position.x + checkDistance * Mathf.Sign(body.velocity.x), transform.position.y, transform.position.z);

        Debug.DrawRay(pos, Vector3.down * height, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(pos, Vector3.down, Mathf.Infinity, layerMask);

        if (hit.collider && hit.distance < height)
        {
            return true;
        }

        return false;
    }

    void Start()
    {
        layerMask = 1 << gameObject.layer | 1 << 2;
        layerMask = ~layerMask;
        body = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        timeBtwShots = startTimeBtwShots;
    }

    void Update()
    {
        if (Scan())
        {
            if (CheckPath())
            {
                if (Vector2.Distance(transform.position, player.position) > stoppingdistance)
                    transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
                else if (Vector2.Distance(transform.position, player.position) < stoppingdistance && (Vector2.Distance(transform.position, player.position)) > retreatDistance) ;
                else if (Vector2.Distance(transform.position, player.position) < retreatDistance)
                    transform.position = Vector2.MoveTowards(transform.position, player.position, -speed * Time.deltaTime);
            }
            if (timeBtwShots <= 0)
            {
                Instantiate(projectile, shootpoint.position, Quaternion.identity);
                timeBtwShots = startTimeBtwShots;
            }
            else
            {
                timeBtwShots -= Time.deltaTime;
            }
        }
        else
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                PlatfomerFlip();
            }
        }    
    }
}