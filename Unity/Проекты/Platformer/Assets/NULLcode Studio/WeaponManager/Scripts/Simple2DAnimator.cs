/********************************************************/
/**  © 2020 NULLcode Studio. All Rights Reserved.
/**  Разработано в рамках проекта: https://null-code.ru
/**  Поддержать нас: https://boosty.to/null-code
/********************************************************/
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class Simple2DAnimator : MonoBehaviour
{
    public Simple2DAnimatorController controller;
    public bool playOnStart = true;
    private Simple2DAnimatorManager manager;
    private SpriteRenderer spriteRenderer;
    private int index, anim, prev, lastKey;
    private ISimple2DAnimator iSimple;
    private float timeout;
    private bool next;
    private string lastID;

    public bool isPlay { get; private set; }

    void Awake()
    {
        iSimple = GetComponent<ISimple2DAnimator>(); // ищем экземпляр на этом объекте, где есть интерфейс (одновременно можно использовать только один)
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        manager = Simple2DAnimatorManager.Add(this); // добавляем этот аниматор в менеджер (на сцене должен присутствовать менеджер аниматоров)
        isPlay = playOnStart;
        lastKey = -1;
    }

    /// <summary>
    /// Получить индекс массива анимации.
    /// </summary>
    /// <param name="id">Идентификатор анимации.</param>
    public int GetAnimationID(string id)
    {
        for (int i = 0; i < controller.animatorData.animatoins.Length; i++)
        {
            if (controller.animatorData.animatoins[i].nameID == id)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Проиграть анимацию, если PlayNext не активно.
    /// </summary>
    /// <param name="id">Идентификатор анимации.</param>
    public void PlayAnimation(string id)
    {
        if (string.IsNullOrEmpty(id) || lastID == id || next) return;

        bool canUse = false;

        for (int i = 0; i < controller.animatorData.animatoins.Length; i++)
        {
            if (controller.animatorData.animatoins[i].nameID == id)
            {
                timeout = 0;
                index = 0;
                anim = i;
                canUse = true;
                isPlay = true;
                break;
            }
        }
        
        if (canUse) lastID = id;
    }

    /// <summary>
    /// Проиграть анимацию и вернуться к предыдущей.
    /// </summary>
    /// <param name="id">Идентификатор анимации.</param>
    public void PlayNext(string id)
    {
        if (string.IsNullOrEmpty(id) || lastID == id) return;

        bool canUse = false;

        for (int i = 0; i < controller.animatorData.animatoins.Length; i++)
        {
            if (controller.animatorData.animatoins[i].nameID == id)
            {
                timeout = 0;
                index = 0;
                prev = anim;
                anim = i;
                canUse = true;
                isPlay = true;
                next = true;
                break;
            }
        }

        if (canUse) lastID = id;
    }

    /// <summary>
    /// Проиграть анимацию, если PlayNext не активно.
    /// </summary>
    /// <param name="id">Идентификатор анимации.</param>
    public void PlayAnimation(int id)
    {
        if (lastKey == id || next) return;

        timeout = 0;
        index = 0;
        anim = id;
        isPlay = true;
        lastKey = id;
    }

    /// <summary>
    /// Проиграть анимацию и вернуться к предыдущей.
    /// </summary>
    /// <param name="id">Идентификатор анимации.</param>
    public void PlayNext(int id)
    {
        if (lastKey == id) return;

        timeout = 0;
        index = 0;
        prev = anim;
        anim = id;
        isPlay = true;
        next = true;
        lastKey = id;
    }

    public void MyUpdate()
    {
        if (!isPlay || controller == null) return;

        timeout += Time.deltaTime;

        if (timeout > controller.animatorData.animatoins[anim].step)
        {
            index++;
            timeout = 0;

            if (index > controller.animatorData.animatoins[anim].sprites.Length - 1)
            {
                index = 0;
                lastID = string.Empty;
                lastKey = -1;
                if (iSimple != null) iSimple.OnAnimatoinEnd(controller.animatorData.animatoins[anim].nameID, anim);

                if (next)
                {
                    anim = prev;
                    timeout = 0;
                    next = false;
                }
                else
                {
                    isPlay = controller.animatorData.animatoins[anim].loop;
                }
            }

            if (controller.animatorData.animatoins[anim].active[index])
            {
                if (iSimple != null) iSimple.OnAnimatoinEvent(controller.animatorData.animatoins[anim].nameID, anim, controller.animatorData.animatoins[anim].sendString[index], controller.animatorData.animatoins[anim].sendFloat[index], controller.animatorData.animatoins[anim].sendInt[index]);
            }

            spriteRenderer.sprite = controller.animatorData.animatoins[anim].sprites[index];
        }        
    }
}

public interface ISimple2DAnimator
{
    void OnAnimatoinEvent(string animatoinName, int animatoinID, string eventString, float eventFloat, int eventInt);
    void OnAnimatoinEnd(string animatoinName, int animatoinID);
}
