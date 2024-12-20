using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip popSound;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPopSound()
    {
        if (audioSource != null)
        {
            audioSource.clip = popSound;
            audioSource.Play();
        }
    }

}
