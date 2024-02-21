using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReticleControl : MonoBehaviour
{
    Image reticle;

    void Start()
    {
        reticle = GetComponent<Image>();
    }

    void Update()
    {
        if(Input.GetButton("Fire1"))
        {
            Color color = reticle.color;
            color.a = 1f;
            reticle.color = color;
        }
        else
        {
            Color color = reticle.color;
            color.a = 0f;
            reticle.color = color;
        }
    }
}
