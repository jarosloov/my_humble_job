/********************************************************/
/**  © 2019 NULLcode Studio. All Rights Reserved.
/**  Разработано в рамках проекта: https://null-code.ru
/**  Поддержать нас: https://boosty.to/null-code
/********************************************************/

using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [Header("Родительский трансформ:")]
    public Transform parent;
    [Header("Здоровье юнита на старте:")]
    public float HP = 100;
    [Header("Размеры UI бара:")]
    public BarType barType;
    [Header("Точка отображение UI бара:")]
    public Transform barPoint; // дочерняя точка привязки бара, как правило размещается над головой юнита
    [Header("Событие смерти:")]
    public UnityEvent eventDie; // можно делать привязку к другим скриптам, которые будут на этом юните, чтобы сообщить им момент, когда здоровье юнита достигнет нуля
    private Bar bar;

    public float barDelta { get; private set; }
    public float currentHP { get { return bar != null ? bar.curHP : 0; } }

    void Start()
    {
        barDelta = Vector3.Distance(parent.position, barPoint.position);
        bar = UIManager.AddEnemy(this);
    }

    void Die()
    {
        bar.MyDestroy();
        eventDie.Invoke();
    }

    public void AdjustHP(float damage)
    {
        bar.Adjust(damage);

        if (bar.curHP <= 0)
        {
            Die();
        }
    }
}
