/********************************************************/
/**  © 2020 NULLcode Studio. All Rights Reserved.
/**  Разработано в рамках проекта: https://null-code.ru
/**  Поддержать нас: https://boosty.to/null-code
/********************************************************/

using UnityEngine;
// внимание (!) интерфейс аниматора работает только с одним скриптом, который должен находится на одном объекте, рядом со скриптом аниматора
public class ExampleControll : MonoBehaviour // интерфейс аниматора (необходим если используются события)
{
    Animator animator;
    public float speed = 1.5f;
    public float acceleration = 100;
    public bool facingRight = true;
    private int attack;
    private bool hit;
    private Rigidbody2D body;
    private Vector3 direction;
    public KeyCode jumpButton = KeyCode.Space;
    [SerializeField]
    public float jumpForce = 5; // сила прыжка
    public float jumpDistance = 0.75f; // расстояние от центра объекта, до поверхности (определяется вручную в зависимости от размеров спрайта)

    private int layerMask;
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;
        animator = GetComponent<Animator>();
        layerMask = 1 << gameObject.layer | 1 << 2;
        layerMask = ~layerMask;
    }

    bool GetJump() // проверяем, есть ли коллайдер под ногами
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, jumpDistance, layerMask);
        if (hit.collider) return true;
        return false;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void Update()
    {
        if (Input.GetKeyDown(jumpButton) && GetJump())
        {
            animator.Play("Jump");
            body.velocity = new Vector2(0, jumpForce);
        }

        float h = Input.GetAxis("Horizontal");
        Debug.Log(h);

        if (h != 0)
        {
            animator.Play("Walk");
        }
        else
        {
            animator.Play("Idle");
        }

        if (h > 0 && !facingRight) Flip(); else if (h < 0 && facingRight) Flip();
    }

    void FixedUpdate()
    {
        body.AddForce(direction * body.mass * speed * acceleration);

        if (Mathf.Abs(body.velocity.x) > speed)
        {
            body.velocity = new Vector2(Mathf.Sign(body.velocity.x) * speed, body.velocity.y);
        }
    }
}
