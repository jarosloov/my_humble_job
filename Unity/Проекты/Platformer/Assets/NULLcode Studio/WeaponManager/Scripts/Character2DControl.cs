/********************************************************/
/**  © 2019 NULLcode Studio. All Rights Reserved.
/**  Разработано в рамках проекта: https://null-code.ru
/**  Поддержать нас: https://boosty.to/null-code
/********************************************************/

using UnityEngine;

public class Character2DControl : MonoBehaviour {

	public float speed = 1.5f; // скорость движения
    public float acceleration = 100; // ускорение
    public float jumpForce = 5; // сила прыжка
    public float jumpDistance = 0.3f; // расстояние от центра объекта, до поверхности (определяется вручную в зависимости от размеров спрайта)
    public KeyCode jumpButton = KeyCode.Space; // клавиша для прыжка

	private Vector3 direction;
	private int layerMask;
	private Rigidbody2D body;
	Animator animator;
	public bool facingRight = true;

	public static Vector3 Spawnpoint = new Vector3(0f, 0f, 0f);
	public Vector3 DefaultSpawnpoint = new Vector3(-40f, -113.6f, -15f);

	void Awake() 
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
		animator = GetComponent<Animator>();
		body = GetComponent<Rigidbody2D>();
		body.freezeRotation = true;
		layerMask = 1 << gameObject.layer | 1 << 2;
		layerMask = ~layerMask;
	}

	bool GetJump() // проверяем, есть ли коллайдер под ногами
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, jumpDistance, layerMask);
		if(hit.collider) return true;
		return false;
	}

	void FixedUpdate()
	{
		body.AddForce(direction * body.mass * speed * acceleration);

		if(Mathf.Abs(body.velocity.x) > speed)
		{
			body.velocity = new Vector2(Mathf.Sign(body.velocity.x) * speed, body.velocity.y);
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, Vector3.down * jumpDistance);
	}

	void Update() 
	{
		if(Input.GetKeyDown(jumpButton) && GetJump())
		{
			animator.Play("Player_Jump");
			body.velocity = new Vector2(0, jumpForce);
		}

		float h = Input.GetAxis("Horizontal");
		if (h != 0)
		{
			animator.Play("Player_Run");
		}
		else
		{
			animator.Play("Player_Idle");
		}

		if (h > 0 && !facingRight) Flip(); else if (h < 0 && facingRight) Flip();
		direction = new Vector2(h, 0); 
	}
	void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
