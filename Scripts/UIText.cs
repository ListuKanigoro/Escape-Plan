using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textAmmo;

    CharacterControl player;
    void Start()
    {
        player = FindObjectOfType<CharacterControl>();
    }

    // Update is called once per frame
    void Update()
    {
        textAmmo.text = player.AmmoBullet.ToString();
    }
}
