using UnityEngine;
using System.Collections;

public class Camera: MonoBehaviour {

    [Header("Скорость сопровождения.")] 
    [Range(0, 1)]   
    public float smooth = 0.1f;                                       
    [Header("Объект сопровождения.")] 
    public GameObject target;                                          
    void FixedUpdate()
    {
        // создается вектор с кооординатами тела
        Vector3 pos = new Vector3( target.transform.position.x, target.transform.position.y, target.transform.position.z );    
        // не моментальный переход в назначенные координаты                                
        transform.position = Vector3.Lerp(transform.position, pos, smooth);
    }
}
