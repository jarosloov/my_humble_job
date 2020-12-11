using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Musicmenu : MonoBehaviour
{
    public AudioSource myFX;
    public AudioClip hoverFx;
    public AudioClip clickFx;

    public void HoverSound()
    {
        myFX.PlayOneShot(hoverFx);
    }

    public void ClickShot()
    {
        myFX.PlayOneShot(clickFx);
    }
}
