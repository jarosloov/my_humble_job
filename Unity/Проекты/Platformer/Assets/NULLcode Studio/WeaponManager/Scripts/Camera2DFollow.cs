/********************************************************/
/**  © 2019 NULLcode Studio. All Rights Reserved.
/**  Разработано в рамках проекта: https://null-code.ru
/**  Поддержать нас: https://boosty.to/null-code
/********************************************************/

using UnityEngine;

public class Camera2DFollow : MonoBehaviour
{
    public float smooth = 2;
    private Transform tr, player;
    private Camera cam;

    void Awake()
    {
        cam = Camera.main;
        tr = transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        tr.position = new Vector3(player.position.x, player.position.y, cam.transform.position.z);
    }

    void PlayerPosition()
    {
        tr.position = Vector3.Lerp(tr.position, new Vector3(player.position.x, player.position.y, cam.transform.position.z), smooth * Time.deltaTime);
    }

    void LateUpdate()
    {
        if (player != null)
        {
            PlayerPosition();
        }
    }
}
