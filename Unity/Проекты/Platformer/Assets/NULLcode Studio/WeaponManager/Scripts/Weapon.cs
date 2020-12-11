/********************************************************/
/**  © 2019 NULLcode Studio. All Rights Reserved.
/**  Разработано в рамках проекта: https://null-code.ru
/**  Поддержать нас: https://boosty.to/null-code
/********************************************************/

using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour {

	[Header("Настройки префаба:")]
	public Rigidbody2D bulletPrefab; // если префаба пули нет, то оружие будет стрелять рейкастом
	public float bulletSpeed = 5; // скорость префаба пули
	[Header("Точка, откуда вылетают пули:")]
	public Transform shootPoint; // точка оружия, откуда должны вылетать пули
    [Header("Скорострельность оружия:")]
    public float fireRate = 1; // скорострельность
    [Header("Точность стрельбы:")]
    [Range(0.75f, 1f)] public float accuracy = 1; // разброс пуль, 1 = 100% точности
    [Header("Настройки патронов и перезарядки:")]
    public int magazine = 15; // объем одного магазина патронов
    public int maxPatrons = 100; // максимальное количество патронов для этого оружия
    public float reloadTime = 2.5f; // время перезарядки в секундах
	[Header("Объект вращения:")]
	public Transform zRotate; // объект вращения, например, само оружие
	public float minAngle = -40; // ограничение по углам
	public float maxAngle = 40;
	
	[Header("Настройки рейкаста:")]
	public float rayDistance = 100; // длинна луча
	public LayerMask rayLayerMask; // маска цели
	public float rayDamage = 15; // урон, наносимый цели

	private int magazineCur, patronsCur, pMax;
	private float invert, timeout, reloadTimeout;
	private Vector3 mouse, direction;
	private bool reload;
    private WeaponManager manager;
    private UIManager uiManager;

    public void Init(WeaponManager weaponManager, int patrons)
    {
        if (patrons > maxPatrons) patrons = maxPatrons;
        CalculatePatrons(patrons);
        uiManager = UIManager.Get();
        manager = weaponManager;
        magazineCur = magazine;
	}

    public void GetWeapon(bool value)
    {
        gameObject.SetActive(value);
        timeout = Mathf.Infinity;
    }

    public void Addpatrons(int value)
    {
        patronsCur += value;

        if (patronsCur > maxPatrons) patronsCur = maxPatrons;
        if (gameObject.activeSelf) UpdateUI();

        if (gameObject.activeSelf && magazineCur <= 0)
        {
            GetReload();
        }
    }

    void CalculatePatrons(int value)
    {
        value -= magazineCur > 0 ? magazine - magazineCur : magazine;

        if (value < 0 && magazine == Mathf.Abs(value))
        {
            pMax = 0;
            magazineCur = 0;
            patronsCur = 0;
            return;
        }
        else if (value < 0 && magazine != Mathf.Abs(value))
        {
            magazineCur = magazine - Mathf.Abs(value);
            pMax = magazineCur;
            patronsCur = 0;
            return;
        }

        magazineCur = magazine;
        patronsCur = value;
        pMax = magazineCur;
    }

    IEnumerator WeaponReload()
	{
		// запуск процесса перезарядки
		reloadTimeout = 0;

		while(true)
		{
			yield return null;

			// процесс перезарядки
			reloadTimeout += Time.deltaTime;
            uiManager.reloadSlider.value = reloadTimeout / reloadTime;

			if(reloadTimeout > reloadTime)
			{
                // перезарядка завершена
                CalculatePatrons(patronsCur);
                timeout = Mathf.Infinity;
                UpdateUI();
				reload = false;
				break;
			}
		}
	}

	void Flip() // отражение по горизонтали
    {
		Vector3 theScale = manager.flipParent.localScale;
		theScale.x *= -1;
		invert *= -1;
        manager.flipParent.localScale = theScale;
	}

	void LookAtMouse() // отслеживание курсора
    {
		Vector3 mousePosMain = Input.mousePosition;
		mousePosMain.z = manager.cam.transform.position.z; 
		mouse = manager.cam.ScreenToWorldPoint(mousePosMain);
		mouse.z = 0;
		Vector3 lookPos = zRotate.position;
		lookPos.z = 0;
		lookPos = mouse - lookPos;
		float angle  = Mathf.Atan2(lookPos.y, lookPos.x * invert) * Mathf.Rad2Deg;
		angle = Mathf.Clamp(angle, minAngle, maxAngle);
		zRotate.rotation = Quaternion.AngleAxis(angle * invert, Vector3.forward);
	}

	void GetReload()
	{
        if (patronsCur <= 0) return;
		reload = true;
		StartCoroutine(WeaponReload());
	}

	public void MyUpdate()
	{
		invert = Mathf.Sign(manager.flipParent.localScale.x);

		if(zRotate != null) LookAtMouse();

		//if(mouse.x < manager.flipParent.position.x && invert == 1) Flip();
		//else if(mouse.x > manager.flipParent.position.x && invert == -1) Flip();

		if(reload) return;

		if(Input.GetMouseButton(0))
		{
			if(magazineCur <= 0)
			{
				GetReload();
				return;
			}

			timeout += Time.deltaTime;
			if(timeout > fireRate)
			{
				timeout = 0;
				Shoot();
			}
		}

		if(Input.GetKeyDown(KeyCode.R) && magazineCur != magazine) // перезарядка "R"
		{
			GetReload();
		}
	}

	void BulletShoot()
	{
		Rigidbody2D clone = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity) as Rigidbody2D;
		clone.velocity = direction * bulletSpeed;
		clone.transform.right = direction;
	}

	void RayShoot()
	{
		RaycastHit2D hit = Physics2D.Raycast(shootPoint.position, direction, rayDistance, rayLayerMask);

		if(hit.transform != null)
		{
			Enemy target = hit.transform.GetComponent<Enemy>();
			if(target != null)  target.AdjustHP(-rayDamage);
		}
	}

	void Shoot()
	{
        direction = (shootPoint.right + (Vector3)(Random.insideUnitCircle * (1f - accuracy))).normalized * invert;
        if (bulletPrefab == null) RayShoot(); else BulletShoot();
        magazineCur--;
        UpdateUI();
    }

    public void UpdateUI()
    {
        uiManager.bulletCounter.text = magazineCur + " / " + patronsCur;
        uiManager.reloadSlider.value = (float)magazineCur / pMax;

        if (magazineCur <= 0)
        {
            GetReload();
        }
    }
}
