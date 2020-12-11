/********************************************************/
/**  © 2020 NULLcode Studio. All Rights Reserved.
/**  Разработано в рамках проекта: https://null-code.ru
/**  Поддержать нас: https://boosty.to/null-code
/********************************************************/
using System.Collections.Generic;
using UnityEngine;

public class Simple2DAnimatorManager : MonoBehaviour
{
    private List<Simple2DAnimator> animators = new List<Simple2DAnimator>();
    private static Simple2DAnimatorManager _inst;

    void Awake()
    {
        _inst = this;
    }

    public static Simple2DAnimatorManager Add(Simple2DAnimator animator)
    {
        return _inst.Add_inst(animator);
    }

    Simple2DAnimatorManager Add_inst(Simple2DAnimator animator)
    {
        animators.Add(animator);
        return this;
    }

    void Update()
    {
        for (int i = 0; i < animators.Count; i++)
        {
            animators[i].MyUpdate();
        }
    }
}
