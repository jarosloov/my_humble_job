/********************************************************/
/**  © 2020 NULLcode Studio. All Rights Reserved.
/**  Разработано в рамках проекта: https://null-code.ru
/**  Поддержать нас: https://boosty.to/null-code
/********************************************************/
using UnityEngine;
[CreateAssetMenu(fileName = "New 2D Animator Controller", menuName = "Custom/Simple 2D Animator Controller...")]
public class Simple2DAnimatorController : ScriptableObject
{
    [HideInInspector] public Simple2DAnimatorData animatorData;
}

[System.Serializable]
public class Simple2DAnimatorData
{
    public Simple2DAnimatorDataList[] animatoins;
}

[System.Serializable]
public class Simple2DAnimatorDataList
{
    public string nameID;
    public float step;
    public bool loop;
    public Sprite[] sprites;
    public string[] sendString;
    public bool[] active;
    public float[] sendFloat;
    public int[] sendInt;
}
