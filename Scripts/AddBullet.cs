using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddBullet : MonoBehaviour
{
    Audio audioPlayer;
    CharacterControl player;

    private void Start() 
    {
        player = FindObjectOfType<CharacterControl>();
        audioPlayer = FindObjectOfType<Audio>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            player.AmmoBullet += 200;
            Destroy(gameObject);
            audioPlayer.Reload();
        }
    }
}
