// Скрипт на перемещение шарика
using UnityEngine;

//эта строчка гарантирует что наш скрипт не завалится 
//ести на плеере будет отсутствовать компонент Rigidbody
[RequireComponent(typeof(Rigidbody))]
public class movi : MonoBehaviour
{
    public float Speed = 10f; // переменная которая хранит скорость перемещения
    public float JumpForce = 300f; // сила прыжка
    // что бы эта переменная работала добавьте тэг "Ground" на вашу поверхность земли
    private bool _isGrounded = true;
    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    //  FixedUpdate срабатывает в определеный мамент не как Update кажды кадр
    void FixedUpdate()
    {
        MovementLogic();
        JumpLogic();
    }

    private void MovementLogic()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(- moveHorizontal, 0.0f, - moveVertical);
        _rb.AddForce(movement * Speed);
    }

    private void JumpLogic()
    {
        if (Input.GetAxis("Jump") > 0)
            _rb.AddForce(Vector3.up * JumpForce);
    }

    void OnCollisionEnter(Collision collision)
    {
        IsGroundedUpate(collision, true);
    }

    void OnCollisionExit(Collision collision)
    {
        IsGroundedUpate(collision, false);
    }

    private void IsGroundedUpate(Collision collision, bool value)
    {
        if (collision.gameObject.tag == ("Ground"))
        {
            _isGrounded = value;
        }
    }
}