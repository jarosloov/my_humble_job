/********************************************************/
/**  © 2019 NULLcode Studio. All Rights Reserved.
/**  Разработано в рамках проекта: https://null-code.ru
/**  Поддержать нас: https://boosty.to/null-code
/********************************************************/

using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Родитель персонажа:")]
    public Transform flipParent;
    [Header("Точка для оружия:")]
    public Transform playerWeaponPoint; // дочерняя точка, куда будет добавляться всё собранное оружие
    private static WeaponManager _inst;
    public Camera cam { get; private set; }
    private List<Weapon> weapons = new List<Weapon>();
    private Weapon lastWeapon;
    private int curWeaponID;

    void Awake()
    {
        cam = Camera.main;
        _inst = this;
    }

    public static void Add(Weapon weapon, int patrons)
    {
        _inst.Add_inst(weapon, patrons);
    }

    void Add_inst(Weapon weapon, int patrons)
    {
        if (weapon == null || AddPatrons(weapon, patrons)) return;
        Weapon clone = Instantiate(weapon, playerWeaponPoint);
        clone.name = weapon.name;
        clone.transform.localPosition = Vector3.zero;
        clone.GetWeapon(lastWeapon != null ? false : true);
        clone.Init(this, patrons);
        weapons.Add(clone);
        if (lastWeapon == null) SwitchWeapon(0);
    }

    bool AddPatrons(Weapon weapon, int patrons)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].name == weapon.name)
            {
                weapons[i].Addpatrons(patrons);
                return true;
            }
        }

        return false;
    }

    void UpdateWeapon()
    {
        if (lastWeapon == null) return;
        lastWeapon.MyUpdate();
    }

    void SwitchWeapon(int value)
    {
        curWeaponID += value;
        if (curWeaponID < 0) curWeaponID = 0;
        if (curWeaponID > weapons.Count - 1) curWeaponID = weapons.Count - 1;
        if (lastWeapon != null && lastWeapon.name == weapons[curWeaponID].name) return;
        if (lastWeapon != null) lastWeapon.GetWeapon(false);
        weapons[curWeaponID].GetWeapon(true);
        lastWeapon = weapons[curWeaponID];
        UIManager.SetWeapon(lastWeapon);
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            SwitchWeapon(1);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            SwitchWeapon(-1);
        }

        UpdateWeapon();
    }
}
