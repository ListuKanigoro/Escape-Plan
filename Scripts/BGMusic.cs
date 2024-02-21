using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusic : MonoBehaviour
{
    AudioSource audioSource;
    void Awake()
    {
        int bgMusic = FindObjectsOfType<BGMusic>().Length;

        if(bgMusic > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = true;
        }
    }

}
