using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEggs : MonoBehaviour
{

    public AudioSource srkSound;
    public AudioSource seeitSound;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            srkSound.Play();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            seeitSound.Play();
        }
    }
}
