/********************************************************/
/**  © 2019 NULLcode Studio. All Rights Reserved.
/**  Разработано в рамках проекта: https://null-code.ru
/**  Поддержать нас: https://boosty.to/null-code
/********************************************************/
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    public GameObject parentObject;
    public RectTransform rectBar, parentTransform;
    public Image image;
    private float delta, timeout;
    public float maxHP { get; private set; }
    public float curHP { get; private set; }
    public float percent { get; private set; }
    public Enemy targetEnemy { get; private set; }
    private UIManager manager;
    private Color clear;
    private bool isShow;

    public void Init(Enemy target, UIManager barManager)
    {
        if (target == null || barManager == null) return;
        manager = barManager;
        parentTransform.sizeDelta = barManager.SetType(target.barType);
        image.color = barManager.barColor;
        targetEnemy = target;
        timeout = barManager.barTimeout;
        maxHP = targetEnemy.HP;
        curHP = targetEnemy.HP;
        delta = Mathf.Abs(parentTransform.sizeDelta.x);
        percent = 1;
        SetTargetRect();
    }

    public void MyDestroy()
    {
        targetEnemy = null;
        parentObject.SetActive(false);
    }

    public void Show(bool value)
    {
        isShow = value;
        parentObject.SetActive(value);
    }

    public void MyUpdate()
    {
        if (isShow)
        {
            PositionUpdate();
            return;
        }

        if (!parentObject.activeSelf) return;

        timeout -= Time.deltaTime;

        if (timeout < 0 || targetEnemy == null)
        {
            parentObject.SetActive(false);
        }

        PositionUpdate();
    }

    void PositionUpdate()
    {
        parentTransform.position = manager.cam.WorldToScreenPoint(targetEnemy.parent.position + Vector3.up * targetEnemy.barDelta);
    }

    void SetTargetRect()
    {
        float offset = -(delta - (delta * percent));
        if (offset > 0) offset = 0;
        rectBar.offsetMax = new Vector2(offset, 0);
        rectBar.offsetMin = Vector2.zero;
    }

    public void Adjust(float value)
    {
        curHP += value;
        if (curHP < 0) curHP = 0;
        if (curHP > maxHP) curHP = maxHP;
        percent = curHP / maxHP;
        timeout = manager.barTimeout;

        if (!parentObject.activeSelf)
        {
            PositionUpdate();
            parentObject.SetActive(true);
        }

        SetTargetRect();
    }
}
