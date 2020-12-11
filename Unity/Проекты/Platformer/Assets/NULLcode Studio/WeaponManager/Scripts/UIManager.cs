/********************************************************/
/**  © 2019 NULLcode Studio. All Rights Reserved.
/**  Разработано в рамках проекта: https://null-code.ru
/**  Поддержать нас: https://boosty.to/null-code
/********************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Цвет бара:")]
    public Color barColor;
    [Header("Префаб бара:")]
    public Bar sample;
    [Header("Время отображения бара, после атаки:")]
    public float barTimeout = 5;
    [Header("Слайдер перезарядки / количества:")]
    public Slider reloadSlider;
    [Header("Количество патронов:")]
    public Text bulletCounter;
    [Header("Текущее оружие:")]
    public Text weaponTitle;
    private List<Bar> bars = new List<Bar>();
    private static UIManager _inst;
    public Camera cam { get; private set; }

    public static UIManager Get()
    {
        return _inst;
    }

    void Awake()
    {
        cam = Camera.main;
        _inst = this;
        gameObject.SetActive(false);
        reloadSlider.interactable = false;
        reloadSlider.value = 0;
        bulletCounter.text = "";
        weaponTitle.text = "";
    }

    public Vector2 SetType(BarType type)
    {
        Vector2 sizeDelta = new Vector2(75, 8);

        switch (type)
        {
            case BarType.Size100x8:
                sizeDelta = new Vector2(100, 8);
                break;

            case BarType.Size150x8:
                sizeDelta = new Vector2(150, 8);
                break;

            case BarType.Size250x8:
                sizeDelta = new Vector2(250, 8);
                break;
        }

        return sizeDelta;
    }

    public static Bar AddEnemy(Enemy target)
    {
        return _inst.AddEnemy_inst(target);
    }

    Bar Get(Enemy target)
    {
        Bar bar = Instantiate(sample, transform);
        bar.parentTransform.localScale = Vector3.one;
        bar.Init(target, this);
        bars.Add(bar);
        return bar;
    }

    Bar AddEnemy_inst(Enemy target)
    {
        for (int i = 0; i < bars.Count; i++)
        {
            if (bars[i].targetEnemy == null)
            {
                bars[i].Init(target, this);
                return bars[i];
            }
        }

        return Get(target);
    }

    public static void SetWeapon(Weapon target)
    {
        _inst.SetWeapon_inst(target);
    }

    void SetWeapon_inst(Weapon target)
    {
        target.UpdateUI();
        weaponTitle.text = target.name;
        gameObject.SetActive(true);
    }

    void Update()
    {
        for (int i = 0; i < bars.Count; i++)
        {
            bars[i].MyUpdate();
        }
    }
}
