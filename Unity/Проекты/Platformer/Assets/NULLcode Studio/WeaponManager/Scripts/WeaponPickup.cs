/********************************************************/
/**  © 2019 NULLcode Studio. All Rights Reserved.
/**  Разработано в рамках проекта: https://null-code.ru
/**  Поддержать нас: https://boosty.to/null-code
/********************************************************/

using UnityEngine;
[RequireComponent(typeof(CircleCollider2D))]
public class WeaponPickup : MonoBehaviour
{
    [Header("Префаб оружия и количество патронов:")]
    public Weapon weaponPrefab;
    public int patrons = 42;

    void OnValidate()
    {
        Collider2D coll = GetComponent<Collider2D>();
        coll.isTrigger = true;
    }

    /*void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag.CompareTo("Player") == 0)
        {
            // если делать программное добавление оружия, например, в начале игры
            // то эту функцию можно делать только (!) через void Start()
            WeaponManager.Add(weaponPrefab, patrons);
            Destroy(gameObject);
        }
    }*/
    private void Start()
    {
        WeaponManager.Add(weaponPrefab, patrons);
        Destroy(gameObject);
    }
}
